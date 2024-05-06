
using CoolBlazor.Infrastructure.Constants.Enums;
using Microsoft.AspNetCore.Components.Forms;

namespace CoolBlazor.Infrastructure.Models.Requests.Upload
{
    public class UploadImageRequest
    {
        public IBrowserFile File { get; set; }
        public string UserId { get; set; }
        public bool IsReplace { get; set; }
        public string FileName { get; set; }
        public string Extension { get; set; }
    }
}