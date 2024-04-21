using System;
using System.ComponentModel.DataAnnotations;

namespace CoolWebApi.Models.Requests.Identity
{
    public class TokenRequest
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}

