
using CoolWebApi.Utils.Constants.Enums;

namespace CoolWebApi.Models.Responses.AWS
{
    public class S3UploadFileResponse
    {
        public string FileName { get; set; }
        public string PathToSave { get; set; }
    }
}