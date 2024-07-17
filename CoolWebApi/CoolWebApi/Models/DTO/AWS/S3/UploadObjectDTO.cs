
using CoolWebApi.Utils.Constants.Enums;

namespace CoolWebApi.Models.DTO.AWS.S3
{
    public class UploadObjectDTO
    {
        public string BucketName { get; set; }
        public string FileName { get; set; }
        public string Prefix { get; set; }
        public string FilePath { get; set; }
        public string UserId { get; set; }
    }
}