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

        public string FullPathGenerator()
        {
            // Get the absolute path of image
            var folder = Infrastructure.Constants.Enums.UploadType.ProfilePicture.ToDescriptionString().Replace('\\', '/');
            return Path.Combine(Directory.GetCurrentDirectory(), folder);
        }

        public async Task<IResult> UploadImageToS3(UploadImageRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync(Routes.AWS.S3.ObjectEndpoints.Upload, request);
            return await response.ToResult();
        }

        public async Task<IResult<string>> SaveImageByStreamLocally(SaveImageDataRequest request)
        {
            var streamData = request.FileData;
            var fileName = request.FileName;
            if (streamData != null)
            {
                var pathToSave = FullPathGenerator();
                // Save image
                if (string.IsNullOrEmpty(pathToSave)) return null;
                if (streamData.Length > 0)
                {
                    // Macos/Linux Only
                    var folder = UploadType.ProfilePicture.ToDescriptionString().Replace('\\', '/');
                    // Macos/Linux Only
                    bool exists = Directory.Exists(pathToSave);
                    if (!exists)
                        Directory.CreateDirectory(pathToSave);
                    var fullPath = Path.Combine(pathToSave, fileName);
                    if (System.IO.File.Exists(fullPath))
                        System.IO.File.Delete(fullPath);
                    if (System.IO.File.Exists(fullPath))
                    {
                        fullPath = NextAvailableFilename(fullPath);
                    }
                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        streamData.CopyTo(stream);
                    }
                    return await Result<string>.SuccessAsync(fullPath);
                }
                else
                {
                    return await Result<string>.FailAsync(string.Empty);
                }
            }
            else
            {
                return await Result<string>.FailAsync(string.Empty);
            }
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

        private static string numberPattern = " ({0})";

        public string NextAvailableFilename(string path)
        {
            // Short-cut if already available
            if (!System.IO.File.Exists(path))
                return path;

            // If path has extension then insert the number pattern just before the extension and return next filename
            if (Path.HasExtension(path))
                return GetNextFilename(path.Insert(path.LastIndexOf(Path.GetExtension(path)), numberPattern));

            // Otherwise just append the pattern to the path and return next filename
            return GetNextFilename(path + numberPattern);
        }

        private static string GetNextFilename(string pattern)
        {
            string tmp = string.Format(pattern, 1);

            if (!System.IO.File.Exists(tmp))
                return tmp; // short-circuit if no matches

            int min = 1, max = 2; // min is inclusive, max is exclusive/untested

            while (System.IO.File.Exists(string.Format(pattern, max)))
            {
                min = max;
                max *= 2;
            }

            while (max != min + 1)
            {
                int pivot = (max + min) / 2;
                if (System.IO.File.Exists(string.Format(pattern, pivot)))
                    min = pivot;
                else
                    max = pivot;
            }

            return string.Format(pattern, max);
        }

        public async Task<Stream> IBrowerFile2Stream(IBrowserFile e)
        {
            // Change IBrowerFile to Stream
            var format = "image/jpg";
            var imageFile = await e.RequestImageFileAsync(format, 400, 400);
            long maxFileSize = 1024 * 1024 * 3; // 5 MB or whatever, don't just use max int
            var readStream = imageFile.OpenReadStream(maxFileSize);
            var buf = new byte[readStream.Length];
            return new MemoryStream(buf);
        }
    }

}