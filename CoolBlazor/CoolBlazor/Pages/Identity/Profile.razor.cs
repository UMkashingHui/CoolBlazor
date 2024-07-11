using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;
using System;
using System.IO;
using System.Threading.Tasks;
using Blazored.FluentValidation;
using CoolBlazor.Infrastructure.Extensions;
using CoolBlazor.Infrastructure.Models.Requests.Identity;
using CoolBlazor.Infrastructure.Constants.Storage;
using CoolBlazor.Infrastructure.Utils.Image;
using CoolBlazor.Infrastructure.Utils.Extensions;
using BootstrapBlazor.Components;
using Blazored.LocalStorage;
using Cropper.Blazor.Models;
using CoolBlazor.Infrastructure.Models.Requests.Identity;
using System.Text;
using CoolBlazor.Infrastructure.Constants.Enums;
using CoolBlazor.Infrastructure.Utils.Wrapper;
using Cropper.Blazor.Components;
using CoolBlazor.Infrastructure.Models.Requests.AWS.S3;
using Cropper.Blazor.Extensions;
using CoolBlazor.Pages.Identity.Dialogs;

namespace CoolBlazor.Pages.Identity
{
    public partial class Profile
    {
        private FluentValidationValidator _fluentValidationValidator;
        private bool Validated => _fluentValidationValidator.Validate(options => { options.IncludeAllRuleSets(); });
        private char _firstLetterOfName;
        private readonly UpdateProfileRequest _profileModel = new();
        private IBrowserFile _file;
        public string ImageDataUrl { get; set; }
        public string CropImagePath { get; set; }
        public string CropImageUrl { get; set; }
        public string UserId { get; set; }


        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await LoadDataAsync();
                StateHasChanged();
            }
        }

        private async Task LoadDataAsync()
        {
            var state = await _stateProvider.GetAuthenticationStateAsync();
            var user = state.User;
            _profileModel.Email = user.GetEmail();
            _profileModel.FirstName = user.GetFirstName();
            _profileModel.LastName = user.GetLastName();
            _profileModel.PhoneNumber = user.GetPhoneNumber();
            UserId = user.GetUserId();
            var imageResponse = await _accountManager.GetProfilePictureAsync(UserId);
            if (imageResponse.Succeeded)
            {
                // var imageResult = await _imageManager.GetImageByKeyFromS3("coolblazorbucket", result.Data);
                ImageDataUrl = "https://coolblazorbucket.s3.ap-southeast-1.amazonaws.com/" + imageResponse.Data;
            }
            else if (_profileModel.FirstName.Length > 0)
            {
                _firstLetterOfName = _profileModel.FirstName[0];
            }
        }

        private async Task UpdateProfileAsync()
        {
            var response = await _accountManager.UpdateProfileAsync(_profileModel);
            if (response.Succeeded)
            {
                await _authenticationManager.Logout();
                _snackBar.Add(_localizer["Your Profile has been updated. Please Login to Continue."], Severity.Success);
                _navigationManager.NavigateTo("/home");
            }
            else
            {
                foreach (var message in response.Messages)
                {
                    _snackBar.Add(message, Severity.Error);
                }
            }
        }


        private async Task CallCropAvatarDialog(IBrowserFile e)
        {
            var fileName = await SaveImageLocally(e, UserId);
            var parameters = new DialogParameters<ImageResizeModal>
            {
                { x => x.CropImageUrl, CropImageUrl },
                { x => x.FileName, fileName },
                { x => x.UserId, UserId }
            };
            var options = new DialogOptions
            {
                CloseButton = true,
                MaxWidth = MaxWidth.Large
            };
            // var dialog = _dialogService.Show<ImageResizeModal>(_localizer["Resize Image"], parameters, options);
            // var result = await dialog.Result;
            // if (!result.Cancelled)
            // {
            //     await CropAndUpload(result.Data.ToString(), fileName);
            // }
            _dialogService.Show<ImageResizeModal>(_localizer["Resize Image"], parameters, options);
        }


        // public async Task CropAndUpload(string croppedData, string fileName)
        // {
        //     // Base64 data to stream
        //     Stream memStream = new MemoryStream(Convert.FromBase64String(croppedData.Decode().base64ImageData));
        //     SaveImageDataRequest request = new SaveImageDataRequest
        //     {
        //         FileData = memStream,
        //         FileName = fileName,
        //         UserId = UserId
        //     };
        //     string relativePath = await _imageOperator.SaveImageByStreamLocally(request);
        //     string fullPath = _imageOperator.FullPathGenerator(relativePath);
        //     // Upload to S3
        //     UploadObjectRequest uploadObjectRequest = new UploadObjectRequest
        //     {
        //         FileName = fileName,
        //         BucketName = "coolblazorbucket",
        //         FilePath = fullPath,
        //         Prefix = $"{UserId}/avatar/",
        //         UserId = UserId
        //     };
        //     var uploadToS3Result = await _imageManager.UploadImageToS3(uploadObjectRequest);
        //     if (uploadToS3Result.Succeeded)
        //     {
        //         UpdateProfilePictureRequest updateProfilePictureRequest = new()
        //         {
        //             Prefix = uploadObjectRequest.Prefix,
        //             FilePath = string.Empty,
        //             BucketName = string.Empty,
        //             FileName = fileName,
        //             UserId = UserId
        //         };
        //         var updateProfilePictureResult = await _accountManager.UpdateProfilePictureAsync(updateProfilePictureRequest);
        //         if (updateProfilePictureResult.Succeeded)
        //         {
        //             if (System.IO.File.Exists(fullPath))
        //                 System.IO.File.Delete(fullPath);
        //             _snackBar.Add(_localizer["Profile picture added."], Severity.Success);
        //             _navigationManager.NavigateTo("/account");
        //         }
        //         else
        //         {
        //             foreach (var error in updateProfilePictureResult.Messages)
        //             {
        //                 _snackBar.Add(error, Severity.Error);
        //             }
        //             _navigationManager.NavigateTo("/account");
        //         }
        //         // It seems that _localStorage cannot access except in OnAfterAsync method.
        //         // await _localStorage.SetItemAsync(StorageConstants.Local.UserImageURL, result.Data);
        //         // var localImageUrl = await _localStorage.GetItemAsStringAsync(StorageConstants.Local.UserImageURL);
        //     }
        //     else
        //     {
        //         foreach (var error in uploadToS3Result.Messages)
        //         {
        //             _snackBar.Add(error, Severity.Error);
        //         }
        //     }
        // }

        private async Task<string> SaveImageLocally(IBrowserFile e, string userId)
        {
            SaveImageDataRequest request = new SaveImageDataRequest();
            var Extension = Path.GetExtension(e.Name);
            string fileName = $"{request.UserId}-{Guid.NewGuid()}{Extension}";

            // Change IBrowerFile to Stream
            var format = "image/jpg";
            var imageFile = await e.RequestImageFileAsync(format, 400, 400);
            long maxFileSize = 1024 * 1024 * 3; // 5 MB or whatever, don't just use max int
            var readStream = imageFile.OpenReadStream(maxFileSize);
            var buf = new byte[readStream.Length];
            var ms = new MemoryStream(buf);
            await readStream.CopyToAsync(ms);
            var buffer = ms.ToArray();
            var streamData = new MemoryStream(buffer);

            request.FileData = streamData;
            request.UserId = userId;
            request.FileName = fileName;
            var relativePath = await _imageOperator.SaveImageByStreamLocally(request);
            CropImageUrl = relativePath;
            int index = CropImageUrl.LastIndexOf('/');
            fileName = CropImageUrl.Substring(index + 1);
            return fileName;
        }

        private async Task DeleteAsync()
        {
            var parameters = new DialogParameters
            {
                {nameof(Shared.Dialogs.DeleteConfirmation.ContentText), $"{string.Format(_localizer["Do you want to delete the profile picture of {0}"], _profileModel.Email)}?"}
            };
            var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true, DisableBackdropClick = true };
            var dialog = _dialogService.Show<Shared.Dialogs.DeleteConfirmation>(_localizer["Delete"], parameters, options);
            var result = await dialog.Result;
            if (!result.Cancelled)
            {
                var request = new UpdateProfilePictureRequest { BucketName = string.Empty, FileName = string.Empty, Prefix = string.Empty, FilePath = string.Empty, UserId = UserId };
                var data = await _accountManager.UpdateProfilePictureAsync(request);
                if (data.Succeeded)
                {
                    await _localStorage.RemoveItemAsync(StorageConstants.Local.UserImageURL);
                    ImageDataUrl = string.Empty;
                    _snackBar.Add(_localizer["Profile picture deleted."], Severity.Success);
                    _navigationManager.NavigateTo("/account", false);
                }
                else
                {
                    _snackBar.Add(_localizer["Profile picture delete fail."], Severity.Error);
                    foreach (var error in data.Messages)
                    {
                        _snackBar.Add(error, Severity.Error);
                    }
                }

            }
        }


    }
}