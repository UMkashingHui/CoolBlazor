using System.Text;
using CoolBlazor.Infrastructure.Managers.Identity.Account;
using CoolBlazor.Infrastructure.Models.Requests.Identity;
using CoolBlazor.Infrastructure.Models.Requests.Upload;
using CoolBlazor.Infrastructure.Models.Responses.Upload;
using CoolBlazor.Infrastructure.Utils.Extensions;
using CoolBlazor.Infrastructure.Utils.Wrapper;
using FluentValidation;
using Microsoft.AspNetCore.Components.Forms;
using System.IO;
using CoolBlazor.Infrastructure.Constants.Enums;
using IResult = CoolBlazor.Infrastructure.Utils.Wrapper.IResult;
using CoolBlazor.Infrastructure.Extensions;

namespace CoolBlazor.Infrastructure.Managers.File
{
    public class ImageManager
    {
        private IAccountManager _accountManager;
        private readonly HttpClient _httpClient = new HttpClient();


        public ImageManager(IAccountManager accountManager, HttpClient httpClient)
        {
            _accountManager = accountManager;
            _httpClient = httpClient;
        }



        public async Task<IResult> UploadImageToS3(UploadImageRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync(Routes.AWS.S3.ObjectEndpoints.Upload, request);
            return await response.ToResult();
        }

        public async Task<IResult> GetImageByKeyFromS3(string bucketName, string key)
        {
            var response = await _httpClient.GetAsync(Routes.AWS.S3.ObjectEndpoints.GetByKey(bucketName, key));
            return await response.ToResult();
        }



        // private async Task<object> SaveImageByDataLocally(SaveImageDataRequest request, string UserId)
        // {
        //     var extension = Path.GetExtension(e.Name);
        //     var _file = e;
        //     var fileName = $"{UserId}-{Guid.NewGuid()}{extension}";
        //     if (_file != null)
        //     {
        //         // Change IBrowerFile to byte[]
        //         var pathToSave = FullPathGenerator();
        //         var format = "image/jpg";
        //         var imageFile = await _file.RequestImageFileAsync(format, 400, 400);
        //         long maxFileSize = 1024 * 1024 * 3; // 5 MB or whatever, don't just use max int
        //         var readStream = imageFile.OpenReadStream(maxFileSize);
        //         var buf = new byte[readStream.Length];
        //         var ms = new MemoryStream(buf);
        //         await readStream.CopyToAsync(ms);
        //         var buffer = ms.ToArray();

        //         // Save image
        //         if (string.IsNullOrEmpty(pathToSave)) return null;
        //         var streamData = new MemoryStream(buffer);
        //         if (streamData.Length > 0)
        //         {
        //             // Macos/Linux Only
        //             var folder = UploadType.ProfilePicture.ToDescriptionString().Replace('\\', '/');
        //             // Macos/Linux Only
        //             bool exists = Directory.Exists(pathToSave);
        //             if (!exists)
        //                 Directory.CreateDirectory(pathToSave);
        //             var fullPath = Path.Combine(pathToSave, fileName);
        //             // var dbPath = Path.Combine(folder, fileName);
        //             if (System.IO.File.Exists(fullPath))
        //                 System.IO.File.Delete(fullPath);
        //             if (System.IO.File.Exists(fullPath))
        //             {
        //                 // dbPath = _imageManager.NextAvailableFilename(dbPath);
        //                 fullPath = _imageManager.NextAvailableFilename(fullPath);
        //             }
        //             using (var stream = new FileStream(fullPath, FileMode.Create))
        //             {
        //                 streamData.CopyTo(stream);
        //             }
        //             return fullPath;
        //             // return dbPath;
        //         }
        //         else
        //         {
        //             return string.Empty;
        //         }
        //     }
        //     return string.Empty;
        // }






    }

}