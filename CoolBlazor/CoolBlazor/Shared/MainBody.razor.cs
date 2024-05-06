using System.Net.Http.Headers;
using CoolBlazor.Infrastructure.Extensions;
using CoolBlazor.Infrastructure.Managers.Identity.Roles;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MudBlazor;

namespace CoolBlazor.Shared
{
    public partial class MainBody
    {
        // private HubConnection hubConnection;
        // public bool IsConnected => hubConnection.State == HubConnectionState.Connected;
        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [Parameter]
        public EventCallback OnDarkModeToggle { get; set; }

        [Parameter]
        public EventCallback<bool> OnRightToLeftToggle { get; set; }

        private bool _drawerOpen = true;

        [Inject] private IRoleManager RoleManager { get; set; }

        private string CurrentUserId { get; set; }
        private string ImageDataUrl { get; set; }
        private string FirstName { get; set; }
        private string Email { get; set; }
        private char FirstLetterOfFirstName { get; set; }
        private bool _rightToLeft = false;

        private async Task RightToLeftToggle()
        {
            var isRtl = await _clientPreferenceManager.ToggleLayoutDirection();
            _rightToLeft = isRtl;

            await OnRightToLeftToggle.InvokeAsync(isRtl);
        }

        public async Task ToggleDarkMode()
        {

            await OnDarkModeToggle.InvokeAsync();
        }



        // protected override async Task OnInitializedAsync()
        // {
        // _rightToLeft = await _clientPreferenceManager.IsRTL();
        // _interceptor.RegisterEvent();

        // try
        // {
        //     var token = await _authenticationManager.TryRefreshToken();
        //     if (!string.IsNullOrEmpty(token))
        //     {
        //         _snackBar.Add(_localizer["Refreshed Token."], Severity.Success);
        //         _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        //     }
        // }
        // catch (Exception ex)
        // {
        //     // Console.WriteLine(ex.Message);
        //     _snackBar.Add(_localizer["You are Logged Out."], Severity.Error);
        //     await _authenticationManager.Logout();
        //     _navigationManager.NavigateTo("/home");
        // }
        // hubConnection = hubConnection.TryInitialize(_navigationManager, _localStorage);
        // Console.WriteLine("AuthToken = " + _localStorage.GetItemAsync<string>(StorageConstants.Local.AuthToken).ToString());
        // await hubConnection.StartAsync();
        // hubConnection.On<string, string, string>(ApplicationConstants.SignalR.ReceiveChatNotification, (message, receiverUserId, senderUserId) =>
        // {
        //     if (CurrentUserId == receiverUserId)
        //     {
        //         _jsRuntime.InvokeAsync<string>("PlayAudio", "notification");
        //         _snackBar.Add(message, Severity.Info, config =>
        //         {
        //             config.VisibleStateDuration = 10000;
        //             config.HideTransitionDuration = 500;
        //             config.ShowTransitionDuration = 500;
        //             config.Action = _localizer["Chat?"];
        //             config.ActionColor = Color.Primary;
        //             config.Onclick = snackbar =>
        //             {
        //                 _navigationManager.NavigateTo($"chat/{senderUserId}");
        //                 return Task.CompletedTask;
        //             };
        //         });
        //     }
        // });
        // hubConnection.On(ApplicationConstants.SignalR.ReceiveRegenerateTokens, async () =>
        // {
        //     try
        //     {
        //         var token = await _authenticationManager.TryForceRefreshToken();
        //         if (!string.IsNullOrEmpty(token))
        //         {
        //             _snackBar.Add(_localizer["Refreshed Token."], Severity.Success);
        //             _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        //         }
        //     }
        //     catch (Exception ex)
        //     {
        //         // Console.WriteLine(ex.Message);
        //         _snackBar.Add(_localizer["You are Logged Out."], Severity.Error);
        //         await _authenticationManager.Logout();
        //         _navigationManager.NavigateTo("/");
        //     }
        // });
        // hubConnection.On<string, string>(ApplicationConstants.SignalR.LogoutUsersByRole, async (userId, roleId) =>
        // {
        //     if (CurrentUserId != userId)
        //     {
        //         var rolesResponse = await RoleManager.GetRolesAsync();
        //         if (rolesResponse.Succeeded)
        //         {
        //             var role = rolesResponse.Data.FirstOrDefault(x => x.Id == roleId);
        //             if (role != null)
        //             {
        //                 var currentUserRolesResponse = await _userManager.GetRolesAsync(CurrentUserId);
        //                 if (currentUserRolesResponse.Succeeded && currentUserRolesResponse.Data.UserRoles.Any(x => x.RoleName == role.Name))
        //                 {
        //                     _snackBar.Add(_localizer["You are logged out because the Permissions of one of your Roles have been updated."], Severity.Error);
        //                     await hubConnection.SendAsync(ApplicationConstants.SignalR.OnDisconnect, CurrentUserId);
        //                     await _authenticationManager.Logout();
        //                     _navigationManager.NavigateTo("/login");
        //                 }
        //             }
        //         }
        //     }
        // });
        // hubConnection.On<string>(ApplicationConstants.SignalR.PingRequest, async (userName) =>
        // {
        //     await hubConnection.SendAsync(ApplicationConstants.SignalR.PingResponse, CurrentUserId, userName);

        // });

        // await hubConnection.SendAsync(ApplicationConstants.SignalR.OnConnect, CurrentUserId);

        // }

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
            if (user == null) return;
            if (user.Identity?.IsAuthenticated == true)
            {
                if (string.IsNullOrEmpty(CurrentUserId))
                {
                    FirstName = user.GetFirstName();
                    CurrentUserId = user.GetUserId();
                    FirstName = user.GetFirstName();
                    if (FirstName.Length > 0)
                    {
                        FirstLetterOfFirstName = FirstName[0];
                    }
                    var imageResponse = await _accountManager.GetProfilePictureAsync(CurrentUserId);
                    if (imageResponse.Succeeded)
                    {
                        ImageDataUrl = imageResponse.Data;
                    }
                    var currentUserResult = await _userManager.GetAsync(CurrentUserId);
                    if (!currentUserResult.Succeeded || currentUserResult.Data == null)
                    {
                        _snackBar.Add(
                            _localizer["You are logged out because the user with your Token has been deleted."],
                            Severity.Error);
                        CurrentUserId = string.Empty;
                        ImageDataUrl = ImageDataUrl;
                        FirstName = FirstName;
                        Email = user.GetEmail();
                        FirstLetterOfFirstName = char.MinValue;
                        await _authenticationManager.Logout();
                    }
                    _snackBar.Add(string.Format(_localizer["Welcome {0}"], FirstName), Severity.Success);
                }
            }
        }

        private void DrawerToggle()
        {
            _drawerOpen = !_drawerOpen;
        }

        private void Logout()
        {
            var parameters = new DialogParameters
            {
                {nameof(Dialogs.Logout.ContentText), $"{_localizer["Logout Confirmation"]}"},
                {nameof(Dialogs.Logout.ButtonText), $"{_localizer["Logout"]}"},
                {nameof(Dialogs.Logout.Color), Color.Error},
                {nameof(Dialogs.Logout.CurrentUserId), CurrentUserId},
                // {nameof(Dialogs.Logout.HubConnection), hubConnection}
            };

            var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.Small, FullWidth = true };

            _dialogService.Show<Dialogs.Logout>(_localizer["Logout"], parameters, options);
        }


    }
}