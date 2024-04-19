using System;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using CoolBlazor.Infrastructure.Managers.Identity.Roles;
using CoolBlazor.Infrastructure.Models.Responses.Identity;
using CoolBlazor.Infrastructure.Constants.Permission;
using CoolBlazor.Infrastructure.Constants.Application;
using CoolBlazor.Infrastructure.Models.Requests.Identity;
using AutoMapper;
using CoolBlazor.Infrastructure.Mappings;

namespace CoolBlazor.Pages.Identity
{
    public partial class RolePermissions
    {
        [Inject] private IRoleManager RoleManager { get; set; }

        // [CascadingParameter] private HubConnection HubConnection { get; set; }
        [Parameter] public string Id { get; set; }
        [Parameter] public string Title { get; set; }
        [Parameter] public string Description { get; set; }

        private PermissionResponse _model;
        private Dictionary<string, List<RoleClaimResponse>> GroupedRoleClaims { get; } = new();
        private IMapper _mapper;
        private RoleClaimResponse _roleClaims = new();
        private RoleClaimResponse _selectedItem = new();
        private string _searchString = "";
        private bool _dense = false;
        private bool _striped = true;
        private bool _bordered = false;
        public bool? _isSelectAll { get; set; } = null;

        private ClaimsPrincipal _currentUser;
        private bool _canEditRolePermissions;
        private bool _canSearchRolePermissions;
        private bool _loaded;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                _currentUser = await _authenticationManager.CurrentUser();
                _canEditRolePermissions = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.RoleClaims.Edit)).Succeeded;
                _canSearchRolePermissions = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.RoleClaims.Search)).Succeeded;

                await GetRolePermissionsAsync();
                _loaded = true;
            }
            StateHasChanged();

            // HubConnection = HubConnection.TryInitialize(_navigationManager, _localStorage);
            // if (HubConnection.State == HubConnectionState.Disconnected)
            // {
            //     await HubConnection.StartAsync();
            // }
        }
        // protected override async Task OnInitializedAsync()
        // {
        //     _currentUser = await _authenticationManager.CurrentUser();
        //     _canEditRolePermissions = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.RoleClaims.Edit)).Succeeded;
        //     _canSearchRolePermissions = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.RoleClaims.Search)).Succeeded;

        //     await GetRolePermissionsAsync();
        //     _loaded = true;
        //     // HubConnection = HubConnection.TryInitialize(_navigationManager, _localStorage);
        //     // if (HubConnection.State == HubConnectionState.Disconnected)
        //     // {
        //     //     await HubConnection.StartAsync();
        //     // }
        // }

        private async Task GetRolePermissionsAsync()
        {
            _mapper = new MapperConfiguration(c => { c.AddProfile<RoleProfile>(); }).CreateMapper();
            var roleId = Id;
            var result = await RoleManager.GetPermissionsAsync(roleId);
            if (result.Succeeded)
            {
                _model = result.Data;
                GroupedRoleClaims.Add(_localizer["All Permissions"], _model.RoleClaims);
                foreach (var claim in _model.RoleClaims)
                {
                    if (GroupedRoleClaims.ContainsKey(claim.Group))
                    {
                        GroupedRoleClaims[claim.Group].Add(claim);
                    }
                    else
                    {
                        GroupedRoleClaims.Add(claim.Group, new List<RoleClaimResponse> { claim });
                    }
                }
                if (_model != null)
                {
                    Description = string.Format(_localizer["Manage {0} {1}'s Permissions"], _model.RoleId, _model.RoleName);
                }
            }
            else
            {
                foreach (var error in result.Messages)
                {
                    _snackBar.Add(error, Severity.Error);
                }
                _navigationManager.NavigateTo("/identity/roles");
            }
        }

        private async Task SaveAsync()
        {
            var request = _mapper.Map<PermissionResponse, PermissionRequest>(_model);
            Console.WriteLine("request.RoleClaims.Count = " + request.RoleClaims.Count);
            var result = await RoleManager.UpdatePermissionsAsync(request);
            if (result.Succeeded)
            {
                _snackBar.Add(result.Messages[0], Severity.Success);
                // await HubConnection.SendAsync(ApplicationConstants.SignalR.SendRegenerateTokens);
                // await HubConnection.SendAsync(ApplicationConstants.SignalR.OnChangeRolePermissions, _currentUser.GetUserId(), request.RoleId);
                _navigationManager.NavigateTo("/identity/roles");
            }
            else
            {
                foreach (var error in result.Messages)
                {
                    _snackBar.Add(error, Severity.Error);
                }
            }
        }

        private async Task SelectAll()
        {
            _isSelectAll = true;
        }

        private bool Search(RoleClaimResponse roleClaims)
        {
            if (string.IsNullOrWhiteSpace(_searchString)) return true;
            if (roleClaims.Value?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true)
            {
                return true;
            }
            if (roleClaims.Description?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true)
            {
                return true;
            }
            return false;
        }

        private Color GetGroupBadgeColor(int selected, int all)
        {
            if (selected == 0)
                return Color.Error;

            if (selected == all)
                return Color.Success;

            return Color.Info;
        }
    }
}