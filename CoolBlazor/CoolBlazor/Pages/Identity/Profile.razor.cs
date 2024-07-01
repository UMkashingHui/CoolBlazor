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
using System.Text;
using CoolBlazor.Infrastructure.Constants.Enums;

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


        private async Task InvokeModal(IBrowserFile e)
        {
            var dbPath = await SaveImageLocally(e, UserId);
            var parameters = new DialogParameters<ImageResizeModal>();
            parameters.Add(x => x.CropImageName, CropImageName);
            parameters.Add(x => x.CropImageUrl, CropImageUrl);
            var options = new DialogOptions
            {
                CloseButton = true,
                MaxWidth = MaxWidth.Large
            };
            var dialog = _dialogService.Show<ImageResizeModal>(_localizer["Resize Image"], parameters, options);
            var result = await dialog.Result;
        }

        private async Task<string> SaveImageLocally(IBrowserFile e, string UserId)
        {
            SaveImageDataRequest request = new SaveImageDataRequest();
            // var pathToSave = _imageManager.FullPathGenerator();
            var Extension = Path.GetExtension(e.Name);
            request.FileData = _imageManager.IBrowerFile2Stream(e).Result;
            request.UserId = UserId;
            var FileName = $"{UserId}-{Guid.NewGuid()}{Extension}";
            request.FileName = FileName;
            CropImageName = FileName;
            CropImageUrl = "images/ProfilePictures/" + CropImageName;
            var result = await _imageManager.SaveImageByStreamLocally(request);
            return result.Data;
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
                var request = new UpdateProfilePictureRequest { Data = new byte[1], FileName = string.Empty, UploadType = Infrastructure.Constants.Enums.UploadType.ProfilePicture, PathToSave = string.Empty, Extension = string.Empty };
                var data = await _accountManager.UpdateProfilePictureAsync(request, UserId);
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