

using CoolBlazor.Infrastructure.Constants.Enums;

namespace CoolBlazor.Infrastructure.Models.Requests.Upload
{
    public class UploadRequest
    {
        public string FileName { get; set; }
        public string Extension { get; set; }
        public UploadType UploadType { get; set; }
        public byte[] Data { get; set; }
        public string PathToSave { get; set; }
    }
}