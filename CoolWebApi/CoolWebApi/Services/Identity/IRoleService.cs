using System.Collections.Generic;
using System.Threading.Tasks;
using CoolWebApi.Models.Requests.Identity;
using CoolWebApi.Models.Responses.Identity;
using CoolWebApi.Utils.Wrapper;

namespace CoolWebApi.Services.Identity
{
    public interface IRoleService : IService
    {
        Task<Result<List<RoleResponse>>> GetAllAsync();

        Task<int> GetCountAsync();

        Task<Result<RoleResponse>> GetByIdAsync(string id);

        Task<Result<string>> SaveAsync(RoleRequest request);

        Task<Result<string>> DeleteAsync(string id);

        Task<Result<PermissionResponse>> GetAllPermissionsAsync(string roleId);

        Task<Result<string>> UpdatePermissionsAsync(PermissionRequest request);
    }
}