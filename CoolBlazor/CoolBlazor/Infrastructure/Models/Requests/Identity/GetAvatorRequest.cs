using System.ComponentModel.DataAnnotations;

namespace CoolBlazor.Infrastructure.Models.Requests.Identity
{
    public class GetAvatorRequest
    {
        [Required]
        public string Key { get; set; }

    }
}