
using CoolWebApi.Utils.Constants.Enums;

namespace CoolWebApi.Models.VO.AWS.S3
{
    public class ObjectVO
    {
        public string Name { get; set; }
        public string PresignedUrl { get; set; }
    }
}