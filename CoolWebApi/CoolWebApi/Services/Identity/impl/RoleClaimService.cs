using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CoolWebApi.Models.Requests.Identity;
using CoolWebApi.Models.Responses.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using CoolWebApi.Services.Identity;
using CoolWebApi.Utils.Wrapper;
using CoolWebApi.Models.Identity;
using MongoDB.Driver;
using CoolWebApi.Utils.Contexts;
using AspNetCore.Identity.Mongo.Mongo;
using Microsoft.AspNetCore.Identity;

namespace CoolWebApi.Services.Identity.impl
{
    public class RoleClaimService : IRoleClaimService
    {
        private readonly IStringLocalizer<RoleClaimService> _localizer;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUserService;
        private readonly MongoIdentityDbContext _db;

        public RoleClaimService(
            IStringLocalizer<RoleClaimService> localizer,
            IMapper mapper,
            ICurrentUserService currentUserService,
            MongoIdentityDbContext db)
        {
            _localizer = localizer;
            _mapper = mapper;
            _currentUserService = currentUserService;
            _db = db;
        }

        public async Task<Result<List<RoleClaimResponse>>> GetAllAsync()
        {
            var roleClaims = _db.RoleClaims;
            var roleClaimsResponse = _mapper.Map<List<RoleClaimResponse>>(roleClaims);
            return await Result<List<RoleClaimResponse>>.SuccessAsync(roleClaimsResponse);
        }

        public async Task<int> GetCountAsync()
        {
            var count = await _db.RoleClaims.EstimatedDocumentCountAsync();
            return (int)count;
        }

        public async Task<Result<RoleClaimResponse>> GetByIdAsync(int id)
        {
            var roleClaim = _db.RoleClaims.FindAsync(x => x.Id.ToString() == id.ToString());
            var roleClaimResponse = _mapper.Map<RoleClaimResponse>(roleClaim);
            return await Result<RoleClaimResponse>.SuccessAsync(roleClaimResponse);
        }

        public async Task<Result<List<RoleClaimResponse>>> GetAllByRoleIdAsync(string roleId)
        {
            var roleClaims = _db.RoleClaims.Find(x => x.RoleId.ToString() == roleId).ToList();
            var roleClaimsResponse = _mapper.Map<List<RoleClaimResponse>>(roleClaims);
            return await Result<List<RoleClaimResponse>>.SuccessAsync(roleClaimsResponse);
        }

        public async Task<Result<string>> SaveAsync(RoleClaimRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.RoleId))
            {
                return await Result<string>.FailAsync(_localizer["Role is required."]);
            }

            if (request.Id == 0)
            {
                var existingRoleClaim =
                    await _db.RoleClaims.FirstOrDefaultAsync((x =>
                            x.RoleId.ToString() == request.RoleId && x.ClaimType == request.Type && x.ClaimValue == request.Value));
                if (existingRoleClaim != null)
                {
                    return await Result<string>.FailAsync(_localizer["Similar Role Claim already exists."]);
                }
                var roleClaim = _mapper.Map<CoolBlazorRoleClaim>(request);
                await _db.RoleClaims.InsertOneAsync(roleClaim);
                return await Result<string>.SuccessAsync(string.Format(_localizer["Role Claim {0} created."], request.Value));
            }
            else
            {
                var existingRoleClaim =
                    await _db.RoleClaims
                        .FirstOrDefaultAsync(x => x.Id.ToString() == request.Id.ToString());
                if (existingRoleClaim == null)
                {
                    return await Result<string>.SuccessAsync(_localizer["Role Claim does not exist."]);
                }
                else
                {
                    var filter = Builders<CoolBlazorRoleClaim>.Filter.Eq("Id", request.Id);
                    var update = Builders<CoolBlazorRoleClaim>.Update
                        .Set(roleClaim => roleClaim.ClaimType, request.Type)
                        .Set(roleClaim => roleClaim.ClaimValue, request.Value)
                        .Set(roleClaim => roleClaim.Group, request.Group)
                        .Set(roleClaim => roleClaim.Description, request.Description)
                        .Set(roleClaim => roleClaim.RoleId.ToString(), request.RoleId);

                    _db.RoleClaims.UpdateOne(filter, update);
                    return await Result<string>.SuccessAsync(string.Format(_localizer["Role Claim {0} for Role {1} updated."], request.Value, existingRoleClaim.Role.Name));
                }
            }
        }

        public async Task<Result<string>> DeleteAsync(int id)
        {
            var existingRoleClaim = await _db.RoleClaims
                .FirstOrDefaultAsync(x => x.Id.ToString() == id.ToString());
            if (existingRoleClaim != null)
            {
                var result = _db.RoleClaims.DeleteOne(x => x.Id.ToString() == id.ToString());
                if (result.DeletedCount == 1)
                {
                    return await Result<string>.SuccessAsync(string.Format(_localizer["Role Claim {0} for {1} Role deleted."], existingRoleClaim.ClaimValue, existingRoleClaim.Role.Name));
                }
                else
                {
                    return await Result<string>.FailAsync(string.Format(_localizer["Role Claim {0} for {1} Role can not be deleted."], existingRoleClaim.ClaimValue, existingRoleClaim.Role.Name));
                }
            }
            else
            {
                return await Result<string>.FailAsync(_localizer["Role Claim does not exist."]);
            }
        }
    }
}