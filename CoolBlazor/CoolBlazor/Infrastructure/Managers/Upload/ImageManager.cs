using System.Text;
using CoolBlazor.Infrastructure.Managers.Identity.Account;
using CoolBlazor.Infrastructure.Models.Requests.Identity;
using CoolBlazor.Infrastructure.Models.Requests.Upload;
using CoolBlazor.Infrastructure.Models.Responses.Upload;
using CoolBlazor.Infrastructure.Utils.Extensions;
using CoolBlazor.Infrastructure.Utils.Wrapper;
using FluentValidation;
using Microsoft.AspNetCore.Components.Forms;

namespace CoolBlazor.Infrastructure.Managers.File
{
    public class ImageManager
    {
        private IAccountManager _accountManager;

        public ImageManager(IAccountManager accountManager)
        {
            _accountManager = accountManager;
        }

        private string FullPathGenerator()
        {
            // Get the absolute path of image
            var folder = Infrastructure.Constants.Enums.UploadType.ProfilePicture.ToDescriptionString().Replace('\\', '/');
            return Path.Combine(Directory.GetCurrentDirectory(), folder);
        }

        public async Task<Result<string>> UploadImage(UploadImageRequest request)
        {
            var _file = request.File;
            if (_file != null)
            {
                var pathToSave = FullPathGenerator();
                var format = "image/jpg";
                var imageFile = await _file.RequestImageFileAsync(format, 400, 400);
                if (request.IsReplace && request.FileName != null)
                {

                }
                long maxFileSize = 1024 * 1024 * 3; // 5 MB or whatever, don't just use max int
                var readStream = imageFile.OpenReadStream(maxFileSize);
                var buf = new byte[readStream.Length];
                var ms = new MemoryStream(buf);
                await readStream.CopyToAsync(ms);
                var buffer = ms.ToArray();

                var updateRequest = new UpdateProfilePictureRequest { Data = buffer, FileName = request.FileName, Extension = request.Extension, UploadType = Infrastructure.Constants.Enums.UploadType.ProfilePicture, PathToSave = pathToSave };
                var result = await _accountManager.UpdateProfilePictureAsync(updateRequest, request.UserId);

                if (result.Succeeded)
                {
                    return await Result<string>.SuccessAsync();
                }
                else
                {
                    return await Result<string>.FailAsync();
                }
            }
            else
            {
                return await Result<string>.FailAsync();
            }
        }

        public async Task<IResult<string>> UploadImageByData(UploadImageDataRequest request)
        {
            var _fileData = request.FileData;
            if (_fileData != null)
            {
                var pathToSave = FullPathGenerator();
                byte[] b = Convert.FromBase64String(_fileData);
                MemoryStream ms = new MemoryStream(b);
                var buffer = ms.ToArray();
                var updateRequest = new UpdateProfilePictureRequest { Data = buffer, FileName = request.FileName, Extension = ".jpg", UploadType = Infrastructure.Constants.Enums.UploadType.ProfilePicture, PathToSave = pathToSave };
                var result = await _accountManager.UpdateProfilePictureAsync(updateRequest, request.UserId);
                return result;
            }
            else
            {
                return await Result<string>.FailAsync();
            }
        }


    }

}