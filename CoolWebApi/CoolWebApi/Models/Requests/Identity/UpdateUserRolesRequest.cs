using System.Collections.Generic;
using CoolWebApi.Models.Responses.Identity;

namespace CoolWebApi.Models.Requests.Identity
{
    public class UpdateUserRolesRequest
    {
        public string UserId { get; set; }
        public IList<UserRoleModel> UserRoles { get; set; }
    }
}