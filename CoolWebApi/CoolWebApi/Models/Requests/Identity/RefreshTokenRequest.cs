using System;
namespace CoolWebApi.Models.Requests.Identity
{
    public class RefreshTokenRequest
    {
        public string Token { get; set; }

        public string RefreshToken { get; set; }
    }
}

