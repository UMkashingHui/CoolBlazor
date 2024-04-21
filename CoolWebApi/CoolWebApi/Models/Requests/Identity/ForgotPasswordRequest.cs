﻿using System.ComponentModel.DataAnnotations;

namespace CoolWebApi.Models.Requests.Identity
{
    public class ForgotPasswordRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}