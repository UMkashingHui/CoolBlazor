using System.Collections.Generic;
using CoolBlazor.Infrastructure.Models.Responses.Identity;

namespace CoolBlazor.Infrastructure.Models.Responses.Identity
{
    public class GetAllUsersResponse
    {
        public IEnumerable<UserResponse> Users { get; set; }
    }
}