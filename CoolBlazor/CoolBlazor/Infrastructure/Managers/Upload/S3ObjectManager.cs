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
using System.Net.Http.Json;

namespace CoolBlazor.Infrastructure.Managers.File
{
    public class S3ObjectManager
    {
        private readonly HttpClient _httpClient = new HttpClient();

        public S3ObjectManager(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IResult> UploadObject(UploadObjectRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync(Routes.AWS.S3.ObjectEndpoints.Upload, request);
            return await response.ToResult();
        }

        public async Task<IResult> DeleteObject(string bucketName, string key)
        {
            var response = await _httpClient.DeleteAsync($"{Routes.AWS.S3.ObjectEndpoints.Delete}?bucketName={bucketName}&key={key}");
            return await response.ToResult();
        }

    }

}