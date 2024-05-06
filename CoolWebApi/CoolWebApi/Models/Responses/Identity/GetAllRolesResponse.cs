using System.Collections.Generic;

namespace CoolWebApi.Models.Responses.Identity
{
    public class GetAllRolesResponse
    {
        public IEnumerable<RoleResponse> Roles { get; set; }
    }
}