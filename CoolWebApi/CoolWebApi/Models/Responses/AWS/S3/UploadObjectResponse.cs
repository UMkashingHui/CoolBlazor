using System.ComponentModel.DataAnnotations;

namespace CoolWebApi.Models.Responses.Identity
{
    public class UploadObjectResponse
    {
        public string Url { get; set; }
        public string Description { get; set; }
    }
}