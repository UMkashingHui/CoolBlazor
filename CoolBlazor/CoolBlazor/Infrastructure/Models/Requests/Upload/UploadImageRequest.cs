﻿
using CoolBlazor.Infrastructure.Constants.Enums;
using Microsoft.AspNetCore.Components.Forms;

namespace CoolBlazor.Infrastructure.Models.Requests.Upload
{
    public class UploadImageRequest
    {
        public string BucketName { get; set; }
        public string FileName { get; set; }
        public string Prefix { get; set; }
        public string FilePath { get; set; }
    }
}