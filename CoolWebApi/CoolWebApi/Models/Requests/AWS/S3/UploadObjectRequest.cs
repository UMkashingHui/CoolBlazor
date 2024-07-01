
using CoolWebApi.Utils.Constants.Enums;

namespace CoolWebApi.Models.Requests.AWS.S3
{
    public class UploadObjectRequest
    {
        public string BucketName { get; set; }
        public string FileName { get; set; }
        public string Prefix { get; set; }
        public string FilePath { get; set; }
    }
}