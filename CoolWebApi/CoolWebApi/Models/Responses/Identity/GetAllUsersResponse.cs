using System.Collections.Generic;

namespace CoolWebApi.Models.Responses.Identity
{
    public class GetAllUsersResponse
    {
        public IEnumerable<UserResponse> Users { get; set; }
    }
}