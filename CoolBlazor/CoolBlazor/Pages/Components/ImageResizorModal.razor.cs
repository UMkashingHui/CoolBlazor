
using System.Security.Claims;
using AutoMapper;
using CoolBlazor.Infrastructure.Constants.Storage;
using CoolBlazor.Infrastructure.Extensions;
using CoolBlazor.Infrastructure.Models.Requests.Identity;
using CoolBlazor.Infrastructure.Models.Requests.Upload;
using CoolBlazor.Infrastructure.Utils.Extensions;
using CoolBlazor.Pages.Identity;
using Cropper.Blazor.Components;
using Cropper.Blazor.Extensions;
using Cropper.Blazor.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.JSInterop;
using MudBlazor;

namespace CoolBlazor.Pages.Components
{
    public partial class ImageResizorModal
    {
        [CascadingParameter] MudDialogInstance MudDialog { get; set; }
        [Parameter] public string CropImageName { get; set; }
        [Parameter] public string CropImageUrl { get; set; }

        public string UserId { get; set; }


        // Crop
        private CropperComponent? cropperComponent = null!;
        private string croppedCanvasDataURL;
        [Inject] private IJSRuntime? JSRuntime { get; set; }

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
            UploadImageDataRequest request = new UploadImageDataRequest();
            GetCroppedCanvasOptions getCroppedCanvasOptions = new GetCroppedCanvasOptions
            {
                MaxHeight = 4096,
                MaxWidth = 4096,
                ImageSmoothingQuality = ImageSmoothingQuality.High.ToEnumString()
            };
            // Get a reference to a JavaScript cropped canvas object.
            CroppedCanvas croppedCanvas = await cropperComponent!.GetCroppedCanvasAsync(getCroppedCanvasOptions);
            // Invoke toDataURL JavaScript function from the canvas object.
            croppedCanvasDataURL = await JSRuntime!.InvokeAsync<string>("window.getEllipseImage",
            croppedCanvas!.JSRuntimeObjectRef);
            request.FileData = croppedCanvasDataURL.Decode().base64ImageData;
            request.FileName = CropImageName;
            request.UserId = UserId;
            var result = await _imageManager.UploadImageByData(request);
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

