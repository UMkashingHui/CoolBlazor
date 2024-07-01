
using CoolBlazor.Infrastructure.Constants.Enums;
using Microsoft.AspNetCore.Components.Forms;

namespace CoolBlazor.Infrastructure.Models.Requests.Upload
{
    public class SaveImageDataRequest
    {
        public Stream FileData { get; set; }
        public string UserId { get; set; }
        public string FileName { get; set; }
    }
}