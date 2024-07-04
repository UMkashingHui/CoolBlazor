
using CoolBlazor.Infrastructure.Constants.Enums;
using Microsoft.AspNetCore.Components.Forms;

namespace CoolBlazor.Infrastructure.Models.Requests.AWS.S3
{
    public class UploadObjectRequest
    {
        public string BucketName { get; set; }
        public string FileName { get; set; }
        public string Prefix { get; set; }
        public string FilePath { get; set; }
        public string UserId { get; set; }
    }
}