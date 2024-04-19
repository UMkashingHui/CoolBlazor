
using CoolBlazor.Infrastructure.Constants.Enums;
using Microsoft.AspNetCore.Components.Forms;

namespace CoolBlazor.Infrastructure.Models.Requests.Upload
{
    public class UploadImageDataRequest
    {
        public string FileData { get; set; }
        public string FileName { get; set; }
        public string UserId { get; set; }
    }
}