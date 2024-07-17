
using CoolWebApi.Utils.Constants.Enums;

namespace CoolWebApi.Models.DTO.AWS.S3
{
    public class DeleteObjectDTO
    {
        public string BucketName { get; set; }
        public string Prefix { get; set; }
        public string FileName { get; set; }
    }
}