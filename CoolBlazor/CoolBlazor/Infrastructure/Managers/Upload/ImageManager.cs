using System.Text;
using CoolBlazor.Infrastructure.Managers.Identity.Account;
using CoolBlazor.Infrastructure.Models.Requests.Identity;
using CoolBlazor.Infrastructure.Models.Requests.Identity;
using CoolBlazor.Infrastructure.Models.Responses.Upload;
using CoolBlazor.Infrastructure.Utils.Extensions;
using CoolBlazor.Infrastructure.Utils.Wrapper;
using FluentValidation;
using Microsoft.AspNetCore.Components.Forms;
using System.IO;
using CoolBlazor.Infrastructure.Constants.Enums;
using IResult = CoolBlazor.Infrastructure.Utils.Wrapper.IResult;
using CoolBlazor.Infrastructure.Extensions;
using CoolBlazor.Infrastructure.Models.Requests.AWS.S3;
using CoolBlazor.Infrastructure.Models.Responses.Identity;

namespace CoolBlazor.Infrastructure.Managers.File
{
    public class ImageManager
    {
        private readonly HttpClient _httpClient = new HttpClient();

        public ImageManager(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IResult> UploadImageToS3(UploadObjectRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync(Routes.AWS.S3.ObjectEndpoints.Upload, request);
            return await response.ToResult();
        }

    }

}