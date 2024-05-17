using AutoMapper;
using CoolWebApi.Models.Requests.Identity;
using CoolWebApi.Models.Responses.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoolWebApi.Services.Identity;
using CoolWebApi.Models.Identity;
using CoolWebApi.Utils.Wrapper;
using CoolWebApi.Utils.Constants.Role;
using CoolWebApi.Utils.Extensions;
using CoolWebApi.Utils.Constants.Permission;
using MongoDB.Bson;

namespace CoolWebApi.Services.Identity.impl
{
    public class RoleService : IRoleService
    {
        private readonly RoleManager<CoolBlazorRole> _roleManager;
        private readonly UserManager<CoolBlazorUser> _userManager;
        private readonly IRoleClaimService _roleClaimService;
        private readonly IStringLocalizer<RoleService> _localizer;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;
        private readonly IRoleStore<CoolBlazorRole> _roleStore;

        public RoleService(
            RoleManager<CoolBlazorRole> roleManager,
            IMapper mapper,
            UserManager<CoolBlazorUser
            > userManager,
            IRoleClaimService roleClaimService,
            IStringLocalizer<RoleService> localizer,
            ICurrentUserService currentUserService)
        {
            _roleManager = roleManager;
            _mapper = mapper;
            _userManager = userManager;
            _roleClaimService = roleClaimService;
            _localizer = localizer;
            _currentUserService = currentUserService;
        }

        public async Task<Result<string>> DeleteAsync(string id)
        {
            var existingRole = await _roleManager.FindByIdAsync(id);
            if (existingRole.Name != RoleConstants.AdministratorRole && existingRole.Name != RoleConstants.BasicRole)
            {
                bool roleIsNotUsed = true;
                var allUsers = _userManager.Users.ToList();

                foreach (var user in allUsers)
                {
                    if (await _userManager.IsInRoleAsync(user, existingRole.Name))
                    {
                        roleIsNotUsed = false;
                    }
                }
                if (roleIsNotUsed)
                {
                    await _roleManager.DeleteAsync(existingRole);
                    return await Result<string>.SuccessAsync(string.Format(_localizer["Role {0} Deleted."], existingRole.Name));
                }
                else
                {
                    return await Result<string>.SuccessAsync(string.Format(_localizer["Not allowed to delete {0} Role as it is being used."], existingRole.Name));
                }
            }
            else
            {
                return await Result<string>.SuccessAsync(string.Format(_localizer["Not allowed to delete {0} Role."], existingRole.Name));
            }
        }

        public async Task<Result<List<RoleResponse>>> GetAllAsync()
        {
            var roles = _roleManager.Roles.ToList();
            var rolesResponse = _mapper.Map<List<RoleResponse>>(roles);
            return await Result<List<RoleResponse>>.SuccessAsync(rolesResponse);
        }

        public async Task<Result<PermissionResponse>> GetAllPermissionsAsync(string roleId)
        {
            var model = new PermissionResponse();
            var allPermissions = GetAllPermissions();
            var role = await _roleManager.FindByIdAsync(roleId);
            if (role != null)
            {
                model.RoleId = role.Id.ToString();
                model.RoleName = role.Name;
                var roleClaimsResult = await _roleClaimService.GetAllByRoleIdAsync(role.Id.ToString());
                if (roleClaimsResult.Succeeded)
                {
                    var roleClaims = roleClaimsResult.Data;
                    var allClaimValues = allPermissions.Select(a => a.Value).ToList();
                    var roleClaimValues = roleClaims.Select(a => a.Value).ToList();
                    var authorizedClaims = allClaimValues.Intersect(roleClaimValues).ToList();
                    foreach (var permission in allPermissions)
                    {
                        if (authorizedClaims.Any(a => a == permission.Value))
                        {
                            permission.Selected = true;
                            var roleClaim = roleClaims.SingleOrDefault(a => a.Value == permission.Value);
                            if (roleClaim?.Description != null)
                            {
                                permission.Description = roleClaim.Description;
                            }
                            if (roleClaim?.Group != null)
                            {
                                permission.Group = roleClaim.Group;
                            }
                        }
                    }
                }
                else
                {
                    model.RoleClaims = new List<RoleClaimResponse>();
                    return await Result<PermissionResponse>.FailAsync(roleClaimsResult.Messages);
                }
            }
            model.RoleClaims = allPermissions;
            return await Result<PermissionResponse>.SuccessAsync(model);
        }

        private List<RoleClaimResponse> GetAllPermissions()
        {
            var allPermissions = new List<RoleClaimResponse>();

            #region GetPermissions

            allPermissions.GetAllPermissions();

            #endregion GetPermissions

            return allPermissions;
        }

        public async Task<Result<RoleResponse>> GetByIdAsync(string id)
        {
            var roles = await _roleManager.Roles.SingleOrDefaultAsync(x => x.Id.ToString() == id);
            var rolesResponse = _mapper.Map<RoleResponse>(roles);
            return await Result<RoleResponse>.SuccessAsync(rolesResponse);
        }

        public async Task<Result<string>> SaveAsync(RoleRequest request)
        {
            if (string.IsNullOrEmpty(request.Id))
            {
                var existingRole = await _roleManager.FindByNameAsync(request.Name);
                if (existingRole != null) return await Result<string>.FailAsync(_localizer["Similar Role already exists."]);
                var response = await _roleManager.CreateAsync(new CoolBlazorRole(request.Name, request.Description));
                if (response.Succeeded)
                {
                    return await Result<string>.SuccessAsync(string.Format(_localizer["Role {0} Created."], request.Name));
                }
                else
                {
                    return await Result<string>.FailAsync(response.Errors.Select(e => _localizer[e.Description].ToString()).ToList());
                }
            }
            else
            {
                var existingRole = await _roleManager.FindByIdAsync(request.Id);
                if (existingRole.Name == RoleConstants.AdministratorRole || existingRole.Name == RoleConstants.BasicRole)
                {
                    return await Result<string>.FailAsync(string.Format(_localizer["Not allowed to modify {0} Role."], existingRole.Name));
                }
                existingRole.Name = request.Name;
                existingRole.NormalizedName = request.Name.ToUpper();
                existingRole.Description = request.Description;
                await _roleManager.UpdateAsync(existingRole);
                return await Result<string>.SuccessAsync(string.Format(_localizer["Role {0} Updated."], existingRole.Name));
            }
        }

        public async Task<Result<string>> UpdatePermissionsAsync(PermissionRequest request)
        {
            try
            {
                var errors = new List<string>();
                var role = await _roleManager.FindByIdAsync(request.RoleId);
                var users = _userManager.Users;
                var userId = _currentUserService.UserId;
                var currentUser = await _userManager.FindByIdAsync(_currentUserService.UserId);
                // var currentUser = _userManager.Users.Where(x => x.Id.Equals(_currentUserService.UserId)).FirstOrDefault();
                if (currentUser == null)
                {
                    return await Result<string>.FailAsync(_localizer["Please login and retry."]);
                }
                if (await _userManager.IsInRoleAsync(currentUser, RoleConstants.AdministratorRole))
                {
                    return await Result<string>.FailAsync(_localizer["Not allowed to modify Permissions for this Role."]);
                }


                var selectedClaims = request.RoleClaims.Where(a => a.Selected == true).ToList();
                if (role.Name == RoleConstants.AdministratorRole)
                {
                    if (!selectedClaims.Any(x => x.Value == Permissions.Roles.View)
                       || !selectedClaims.Any(x => x.Value == Permissions.RoleClaims.View)
                       || !selectedClaims.Any(x => x.Value == Permissions.RoleClaims.Edit))
                    {
                        return await Result<string>.FailAsync(string.Format(
                            _localizer["Not allowed to deselect {0} or {1} or {2} for this Role."],
                            Permissions.Roles.View, Permissions.RoleClaims.View, Permissions.RoleClaims.Edit));
                    }
                }

                var claims = await _roleManager.GetClaimsAsync(role);
                foreach (var claim in claims)
                {
                    await _roleManager.RemoveClaimAsync(role, claim);
                }
                foreach (var claim in selectedClaims)
                {
                    var addResult = await _roleManager.AddPermissionClaim(role, claim.Value);
                    if (!addResult.Succeeded)
                    {
                        errors.AddRange(addResult.Errors.Select(e => _localizer[e.Description].ToString()));
                    }
                }

                var addedClaims = await _roleClaimService.GetAllByRoleIdAsync(role.Id.ToString());
                if (addedClaims.Succeeded)
                {
                    foreach (var claim in selectedClaims)
                    {
                        var addedClaim = addedClaims.Data.SingleOrDefault(x => x.Type == claim.Type && x.Value == claim.Value);
                        if (addedClaim != null)
                        {
                            claim.Id = addedClaim.Id;
                            claim.RoleId = addedClaim.RoleId;
                            var saveResult = await _roleClaimService.SaveAsync(claim);
                            if (!saveResult.Succeeded)
                            {
                                errors.AddRange(saveResult.Messages);
                            }
                        }
                    }
                }
                else
                {
                    errors.AddRange(addedClaims.Messages);
                }

                if (errors.Any())
                {
                    return await Result<string>.FailAsync(errors);
                }

                return await Result<string>.SuccessAsync(_localizer["Permissions Updated."]);
            }
            catch (Exception ex)
            {
                return await Result<string>.FailAsync(ex.Message);
            }
        }

        public async Task<int> GetCountAsync()
        {
            return _roleManager.Roles.Count();
        }
    }
}