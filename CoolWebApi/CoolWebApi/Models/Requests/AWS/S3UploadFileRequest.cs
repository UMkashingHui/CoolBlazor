
using CoolWebApi.Utils.Constants.Enums;

namespace CoolWebApi.Models.Requests.AWS
{
    public class S3UploadFileRequest
    {
        public string BucketName { get; set; }
        public string Key { get; set; }
        public Stream InputStream { get; set; }
    }
}