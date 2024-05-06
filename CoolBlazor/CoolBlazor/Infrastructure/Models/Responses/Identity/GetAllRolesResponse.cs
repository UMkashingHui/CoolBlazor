using System.Collections.Generic;

namespace CoolBlazor.Infrastructure.Models.Responses.Identity
{
    public class GetAllRolesResponse
    {
        public IEnumerable<RoleResponse> Roles { get; set; }
    }
}