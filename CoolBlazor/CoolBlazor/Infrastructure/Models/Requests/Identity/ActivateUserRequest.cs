using CoolBlazor.Infrastructure.Models.Responses.Identity;
using System.Collections.Generic;

namespace CoolBlazor.Infrastructure.Models.Requests.Identity
{
    public class ActivateUserRequest
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
    }
}