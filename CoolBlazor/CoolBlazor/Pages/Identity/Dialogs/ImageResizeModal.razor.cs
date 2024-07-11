using CoolBlazor.Infrastructure.Models.Requests.AWS.S3;
using CoolBlazor.Infrastructure.Models.Requests.Identity;
using Cropper.Blazor.Components;
using Cropper.Blazor.Extensions;
using Cropper.Blazor.Models;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace CoolBlazor.Pages.Identity.Dialogs
{
    public partial class ImageResizeModal
    {
        [CascadingParameter] MudDialogInstance MudDialog { get; set; }

        [Parameter] public string CropImageUrl { get; set; }
        [Parameter] public string UserId { get; set; }
        [Parameter] public string FileName { get; set; }

        private CropperComponent? cropperComponent = null;
        // Minimum relative sizes
        private static decimal minCropBoxWidth = 200;
        private static decimal minCropBoxHeight = 200;
        private static bool cropBoxResizable = false;
        private static decimal height = 200;
        private static decimal width = 200;
        private static SetDataOptions setDataOptions = new SetDataOptions
        {
            Height = height,
            Width = width
        };
        private Options cropperOptions = new Options
        {
            MinCropBoxWidth = minCropBoxWidth,
            MinCropBoxHeight = minCropBoxHeight,
            CropBoxResizable = cropBoxResizable,
            // ViewMode = ViewMode.Vm2,
            SetDataOptions = setDataOptions,
            Preview = ".img-example-preview"
        };
        public async Task Submit()
        {
            GetCroppedCanvasOptions getCroppedCanvasOptions = new GetCroppedCanvasOptions
            {
                MaxHeight = 4096,
                MaxWidth = 4096,
                ImageSmoothingQuality = ImageSmoothingQuality.High.ToEnumString()
            };
            // Get a reference to a JavaScript cropped canvas object.
            string croppedData = await cropperComponent.GetCroppedCanvasDataURLAsync(getCroppedCanvasOptions);
            CropAndUpload(croppedData, FileName);
            // MudDialog.Close(DialogResult.Ok(croppedData));
            MudDialog.Close();
        }
        void Cancel() => MudDialog.Cancel();

        public async Task CropAndUpload(string croppedData, string fileName)
        {
            // Base64 data to stream
            Stream memStream = new MemoryStream(Convert.FromBase64String(croppedData.Decode().base64ImageData));
            SaveImageDataRequest request = new SaveImageDataRequest
            {
                FileData = memStream,
                FileName = fileName,
                UserId = UserId
            };
            string relativePath = await _imageOperator.SaveImageByStreamLocally(request);
            string fullPath = _imageOperator.FullPathGenerator(relativePath);
            // Upload to S3
            UploadObjectRequest uploadObjectRequest = new UploadObjectRequest
            {
                FileName = fileName,
                BucketName = "coolblazorbucket",
                FilePath = fullPath,
                Prefix = $"{UserId}/avatar/",
                UserId = UserId
            };
            var uploadToS3Result = await _imageManager.UploadImageToS3(uploadObjectRequest);
            if (uploadToS3Result.Succeeded)
            {
                // TODO Delete the object
                

                UpdateProfilePictureRequest updateProfilePictureRequest = new()
                {
                    Prefix = uploadObjectRequest.Prefix,
                    FilePath = string.Empty,
                    BucketName = string.Empty,
                    FileName = fileName,
                    UserId = UserId
                };
                var updateProfilePictureResult = await _accountManager.UpdateProfilePictureAsync(updateProfilePictureRequest);
                if (updateProfilePictureResult.Succeeded)
                {
                    if (System.IO.File.Exists(fullPath))
                        System.IO.File.Delete(fullPath);
                    _snackBar.Add(_localizer["Profile picture added."], Severity.Success);
                    _navigationManager.NavigateTo("/account", true);
                }
                else
                {
                    foreach (var error in updateProfilePictureResult.Messages)
                    {
                        _snackBar.Add(error, Severity.Error);
                    }
                    _navigationManager.NavigateTo("/account", true);
                }
                // It seems that _localStorage cannot access except in OnAfterAsync method.
                // await _localStorage.SetItemAsync(StorageConstants.Local.UserImageURL, result.Data);
                // var localImageUrl = await _localStorage.GetItemAsStringAsync(StorageConstants.Local.UserImageURL);
            }
            else
            {
                foreach (var error in uploadToS3Result.Messages)
                {
                    _snackBar.Add(error, Severity.Error);
                }
            }
        }
    }


}