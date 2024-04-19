using CoolBlazor.Infrastructure.Models.Responses.Identity;
using System.Collections.Generic;

namespace CoolBlazor.Infrastructure.Models.Requests.Identity
{
    public class UpdateUserRolesRequest
    {
        public string UserId { get; set; }
        public IList<UserRoleModel> UserRoles { get; set; }
    }
}