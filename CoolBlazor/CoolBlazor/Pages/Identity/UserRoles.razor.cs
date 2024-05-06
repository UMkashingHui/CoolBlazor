using System;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using CoolBlazor.Infrastructure.Models.Responses.Identity;
using CoolBlazor.Infrastructure.Constants.Permission;
using CoolBlazor.Infrastructure.Models.Requests.Identity;
using CoolBlazor.Infrastructure.Constants.Storage;
using System.Net.Http.Headers;

namespace CoolBlazor.Pages.Identity
{
    public partial class UserRoles
    {
        [Parameter] public string Id { get; set; }
        [Parameter] public string Title { get; set; }
        [Parameter] public string Description { get; set; }
        public List<UserRoleModel> UserRolesList { get; set; } = new();

        private UserRoleModel _userRole = new();
        private string _searchString = "";
        private bool _dense = false;
        private bool _striped = true;
        private bool _bordered = false;

        private ClaimsPrincipal _currentUser;
        private bool _canEditUsers;
        private bool _canSearchRoles;
        private bool _loaded;

        // protected override async Task OnInitializedAsync()
        // {
        //     _currentUser = await _authenticationManager.CurrentUser();
        //     _canEditUsers = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.Users.Edit)).Succeeded;
        //     _canSearchRoles = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.Roles.Search)).Succeeded;

        //     var userId = Id;
        //     var result = await _userManager.GetAsync(userId);
        //     if (result.Succeeded)
        //     {
        //         var user = result.Data;
        //         if (user != null)
        //         {
        //             Title = $"{user.FirstName} {user.LastName}";
        //             Description = string.Format(_localizer["Manage {0} {1}'s Roles"], user.FirstName, user.LastName);
        //             var response = await _userManager.GetRolesAsync(user.Id);
        //             UserRolesList = response.Data.UserRoles;
        //         }
        //     }

        //     _loaded = true;
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
            _currentUser = await _authenticationManager.CurrentUser();
            _canEditUsers = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.Users.Edit)).Succeeded;
            _canSearchRoles = (await _authorizationService.AuthorizeAsync(_currentUser, Permissions.Roles.Search)).Succeeded;

            var userId = Id;
            var result = await _userManager.GetAsync(userId);
            var savedToken = await _localStorage.GetItemAsync<string>(StorageConstants.Local.AuthToken);
            if (result.Succeeded)
            {
                var user = result.Data;
                if (user != null)
                {
                    Title = $"{user.FirstName} {user.LastName}";
                    Description = string.Format(_localizer["Manage {0} {1}'s Roles"], user.FirstName, user.LastName);
                    _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", savedToken);
                    await _localStorage.SetItemAsync<string>(StorageConstants.Local.AuthToken, savedToken);
                    var response = await _userManager.GetRolesAsync(user.Id);
                    StateHasChanged();
                    UserRolesList = response.Data.UserRoles;
                }
            }

            _loaded = true;
        }

        private async Task SaveAsync()
        {
            var request = new UpdateUserRolesRequest()
            {
                UserId = Id,
                UserRoles = UserRolesList
            };
            var result = await _userManager.UpdateRolesAsync(request);
            if (result.Succeeded)
            {
                _snackBar.Add(result.Messages[0], Severity.Success);
                _navigationManager.NavigateTo("/identity/users");
            }
            else
            {
                foreach (var error in result.Messages)
                {
                    _snackBar.Add(error, Severity.Error);
                }
            }
        }

        private bool Search(UserRoleModel userRole)
        {
            if (string.IsNullOrWhiteSpace(_searchString)) return true;
            if (userRole.RoleName?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true)
            {
                return true;
            }
            if (userRole.RoleDescription?.Contains(_searchString, StringComparison.OrdinalIgnoreCase) == true)
            {
                return true;
            }
            return false;
        }
    }
}