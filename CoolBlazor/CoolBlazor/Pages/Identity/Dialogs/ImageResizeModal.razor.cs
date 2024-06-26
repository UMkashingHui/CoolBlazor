using System.Text;
using CoolBlazor.Infrastructure.Extensions;
using CoolBlazor.Infrastructure.Models.Requests.Upload;
using Cropper.Blazor.Components;
using Cropper.Blazor.Extensions;
using Cropper.Blazor.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;

namespace CoolBlazor.Pages.Identity.Dialogs
{
    public partial class ImageResizeModal
    {
        [CascadingParameter] MudDialogInstance MudDialog { get; set; }
        [Parameter] public string CropImageName { get; set; }
        [Parameter] public string CropImageUrl { get; set; }

        public string UserId { get; set; }
        private CropperComponent? cropperComponent = null!;
        // Minimum relative sizes
        private static decimal minCropBoxWidth = 200;
        private static decimal minCropBoxHeight = 200;
        private static bool cropBoxResizable = false;
        private static decimal height = 200;
        private static decimal width = 200;


        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                var state = await _stateProvider.GetAuthenticationStateAsync();
                var user = state.User;
                UserId = user.GetUserId();
                StateHasChanged();
            }
        }

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


        public async Task GetCroppedCanvasDataURLAsync()
        {
            SaveImageDataRequest request = new SaveImageDataRequest();
            GetCroppedCanvasOptions getCroppedCanvasOptions = new GetCroppedCanvasOptions
            {
                MaxHeight = 4096,
                MaxWidth = 4096,
                ImageSmoothingQuality = ImageSmoothingQuality.High.ToEnumString()
            };
            // Get a reference to a JavaScript cropped canvas object.
            // CroppedCanvas croppedCanvas = await cropperComponent!.GetCroppedCanvasAsync(getCroppedCanvasOptions);
            string croppedData = await cropperComponent.GetCroppedCanvasDataURLAsync(getCroppedCanvasOptions);

            // request.FileData = croppedData.Decode().base64ImageData;
            byte[] byteArray = Encoding.ASCII.GetBytes(croppedData);
            MemoryStream stream = new MemoryStream(byteArray);
            request.FileData = stream;
            request.FileName = CropImageName;
            request.UserId = UserId;
            var result = await _imageManager.SaveImageByStreamLocally(request);
            if (result.Succeeded)
            {
                // It seems that _localStorage cannot access except in OnAfterAsync method.
                // await _localStorage.SetItemAsync(StorageConstants.Local.UserImageURL, result.Data);
                // var localImageUrl = await _localStorage.GetItemAsStringAsync(StorageConstants.Local.UserImageURL);
                _snackBar.Add(_localizer["Profile picture added."], Severity.Success);
                _navigationManager.NavigateTo("/account", true);
            }
            else
            {
                foreach (var error in result.Messages)
                {
                    _snackBar.Add(error, Severity.Error);
                }
            }
        }
        void Cancel() => MudDialog.Cancel();
    }
}

