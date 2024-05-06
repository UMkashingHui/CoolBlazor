using System.ComponentModel.DataAnnotations;

namespace CoolBlazor.Infrastructure.Models.Requests.Identity
{
    public class ForgotPasswordRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}