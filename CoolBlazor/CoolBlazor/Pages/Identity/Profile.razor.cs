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
using CoolBlazor.Infrastructure.Utils.Extensions;
using BootstrapBlazor.Components;
using Blazored.LocalStorage;
using Cropper.Blazor.Models;
using CoolBlazor.Infrastructure.Models.Requests.Upload;
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
        public string CropImageName { get; set; }
        public string CropImageUrl { get; set; }

        public string UserId { get; set; }

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

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await LoadDataAsync();
                StateHasChanged();
            }
        }
        // protected override async Task OnInitializedAsync()
        // {
        //     await LoadDataAsync();
        //     StateHasChanged();
        // }

        private async Task LoadDataAsync()
        {
            var state = await _stateProvider.GetAuthenticationStateAsync();
            var user = state.User;
            _profileModel.Email = user.GetEmail();
            _profileModel.FirstName = user.GetFirstName();
            _profileModel.LastName = user.GetLastName();
            _profileModel.PhoneNumber = user.GetPhoneNumber();

            UserId = user.GetUserId();
            var data = await _accountManager.GetProfilePictureAsync(UserId);
            if (data.Succeeded)
            {
                ImageDataUrl = data.Data;
            }
            if (_profileModel.FirstName.Length > 0)
            {
                _firstLetterOfName = _profileModel.FirstName[0];
            }
        }


        private async Task InvokeModal(IBrowserFile e)
        {
            await UploadToCrop(e, UserId);
            var parameters = new DialogParameters<ImageResizorModal>();
            parameters.Add(x => x.CropImageName, CropImageName);
            parameters.Add(x => x.CropImageUrl, CropImageUrl);
            var options = new DialogOptions
            {
                CloseButton = true,
                MaxWidth = MaxWidth.Large
            };
            _dialogService.Show<ImageResizorModal>(_localizer["Resize Image"], parameters, options);
        }

        private async Task UploadToCrop(IBrowserFile e, string userId)
        {
            UploadImageRequest request = new UploadImageRequest();
            request.File = e;
            var extension = Path.GetExtension(e.Name);
            request.FileName = $"{request.UserId}-{Guid.NewGuid()}{extension}";
            request.IsReplace = false;
            request.UserId = userId;
            request.Extension = extension;
            var result = await _imageManager.UploadImage(request);
            if (result.Succeeded)
            {
                CropImageName = request.FileName;
                CropImageUrl = "images/ProfilePictures/" + CropImageName;
            }
            else
            {
                foreach (var error in result.Messages)
                {
                    _snackBar.Add(error, Severity.Error);
                }
            }
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
                var request = new UpdateProfilePictureRequest { Data = null, FileName = string.Empty, UploadType = Infrastructure.Constants.Enums.UploadType.ProfilePicture };
                var data = await _accountManager.UpdateProfilePictureAsync(request, UserId);
                if (data.Succeeded)
                {
                    await _localStorage.RemoveItemAsync(StorageConstants.Local.UserImageURL);
                    ImageDataUrl = string.Empty;
                    _snackBar.Add(_localizer["Profile picture deleted."], Severity.Success);
                    _navigationManager.NavigateTo("/account", true);
                }
                else
                {
                    foreach (var error in data.Messages)
                    {
                        _snackBar.Add(error, Severity.Error);
                    }
                }
            }
        }
    }
}