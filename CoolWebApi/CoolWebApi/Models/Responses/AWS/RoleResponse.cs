using System.ComponentModel.DataAnnotations;

namespace CoolWebApi.Models.Responses.Identity
{
    public class BucketResponse
    {
        public string Id { get; set; }

        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
    }
}