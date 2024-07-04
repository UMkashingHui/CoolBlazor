// using System.Text;
// using CoolBlazor.Infrastructure.Extensions;
// using CoolBlazor.Infrastructure.Models.Requests.AWS.S3;
// using CoolBlazor.Infrastructure.Models.Requests.Identity;
// using CoolBlazor.Infrastructure.Models.Requests.Identity;
// using Cropper.Blazor.Components;
// using Cropper.Blazor.Extensions;
// using Cropper.Blazor.Models;
// using Microsoft.AspNetCore.Components;
// using Microsoft.AspNetCore.Components.Forms;
// using MudBlazor;

// namespace CoolBlazor.Pages.Identity.Dialogs
// {
//     public partial class ImageResizeModal
//     {
//         [CascadingParameter] MudDialogInstance MudDialog { get; set; }
//         [Parameter] public string CropImageName { get; set; }
//         [Parameter] public string CropImageUrl { get; set; }

//         public string UserId { get; set; }
//         private CropperComponent? cropperComponent = null!;
//         // Minimum relative sizes
//         private static decimal minCropBoxWidth = 200;
//         private static decimal minCropBoxHeight = 200;
//         private static bool cropBoxResizable = false;
//         private static decimal height = 200;
//         private static decimal width = 200;


//         protected override async Task OnAfterRenderAsync(bool firstRender)
//         {
//             if (firstRender)
//             {
//                 var state = await _stateProvider.GetAuthenticationStateAsync();
//                 var user = state.User;
//                 UserId = user.GetUserId();
//                 StateHasChanged();
//             }
//         }

//         private static SetDataOptions setDataOptions = new SetDataOptions
//         {
//             Height = height,
//             Width = width
//         };

//         private Options cropperOptions = new Options
//         {
//             MinCropBoxWidth = minCropBoxWidth,
//             MinCropBoxHeight = minCropBoxHeight,
//             CropBoxResizable = cropBoxResizable,
//             // ViewMode = ViewMode.Vm2,
//             SetDataOptions = setDataOptions,
//             Preview = ".img-example-preview"
//         };



//         public async Task GetCroppedCanvasDataURLAsync()
//         {
//             GetCroppedCanvasOptions getCroppedCanvasOptions = new GetCroppedCanvasOptions
//             {
//                 MaxHeight = 4096,
//                 MaxWidth = 4096,
//                 ImageSmoothingQuality = ImageSmoothingQuality.High.ToEnumString()
//             };
//             // Get a reference to a JavaScript cropped canvas object.
//             string croppedData = await cropperComponent.GetCroppedCanvasDataURLAsync(getCroppedCanvasOptions);
//             // Base64 data to stream
//             Stream memStream = new MemoryStream(Convert.FromBase64String(croppedData.Decode().base64ImageData));
//             SaveImageDataRequest request = new SaveImageDataRequest
//             {
//                 FileData = memStream,
//                 FileName = CropImageName,
//                 UserId = UserId
//             };
//             var fullPath = await _imageOperator.SaveImageByStreamLocally(request);
//             // Upload to S3
//             UploadObjectRequest uploadObjectRequest = new UploadObjectRequest
//             {
//                 FileName = CropImageName,
//                 BucketName = "coolblazorbucket",
//                 FilePath = fullPath,
//                 Prefix = $"{UserId}/avatar/",
//                 UserId = UserId
//             };
//             var uploadToS3Result = await _imageManager.UploadImageToS3(uploadObjectRequest);
//             if (uploadToS3Result.Succeeded)
//             {
//                 UpdateProfilePictureRequest updateProfilePictureRequest = new()
//                 {
//                     Prefix = uploadObjectRequest.Prefix,
//                     FilePath = string.Empty,
//                     BucketName = string.Empty,
//                     FileName = CropImageName,
//                     UserId = UserId
//                 };
//                 var updateProfilePictureResult = await _accountManager.UpdateProfilePictureAsync(updateProfilePictureRequest);
//                 if (updateProfilePictureResult.Succeeded)
//                 {
//                     if (System.IO.File.Exists(fullPath))
//                         System.IO.File.Delete(fullPath);
//                     _snackBar.Add(_localizer["Profile picture added."], Severity.Success);
//                     _navigationManager.NavigateTo("/account");
//                 }
//                 else
//                 {
//                     foreach (var error in updateProfilePictureResult.Messages)
//                     {
//                         _snackBar.Add(error, Severity.Error);
//                     }
//                     _navigationManager.NavigateTo("/account");
//                 }
//                 // It seems that _localStorage cannot access except in OnAfterAsync method.
//                 // await _localStorage.SetItemAsync(StorageConstants.Local.UserImageURL, result.Data);
//                 // var localImageUrl = await _localStorage.GetItemAsStringAsync(StorageConstants.Local.UserImageURL);
//             }
//             else
//             {
//                 foreach (var error in uploadToS3Result.Messages)
//                 {
//                     _snackBar.Add(error, Severity.Error);
//                 }
//             }
//         }
//         void Cancel() => MudDialog.Cancel();
//     }
// }

