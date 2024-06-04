using System.Collections.Generic;
using CoolWebApi.Models.Responses.Identity;

namespace CoolWebApi.Models.Requests.Identity
{
    public class ActivateUserRequest
    {
        public string UserId { get; set; }
        public string UserName { get; set; }
    }
}