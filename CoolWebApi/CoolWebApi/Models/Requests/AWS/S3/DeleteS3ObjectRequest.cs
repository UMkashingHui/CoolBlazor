
using CoolWebApi.Utils.Constants.Enums;

namespace CoolWebApi.Models.Requests.AWS.S3
{
    public class DeleteS3ObjectRequest
    {
        public string BucketName { get; set; }
        public string Prefix { get; set; }
        public string FileName { get; set; }
    }
}