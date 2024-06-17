using Blazored.FluentValidation;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using System.Security.Claims;
using System.Text.RegularExpressions;
using CoolBlazor.Infrastructure.Models.Requests.Identity;

namespace CoolBlazor.Pages.Authentication
{
    public partial class Login
    {
        // Refer to BlazorHero
        private FluentValidationValidator _fluentValidationValidator;
        private bool Validated => _fluentValidationValidator.Validate(options => { options.IncludeAllRuleSets(); });
        private TokenRequest _tokenModel = new();
        private bool _passwordVisibility;
        private InputType _passwordInput = InputType.Password;
        private string _passwordInputIcon = Icons.Material.Filled.VisibilityOff;
        public string ReadOnlyEmail { get; set; } = "SuperUser@163.com";
        public string ReadOnlyPWD { get; set; } = "123Pa$$word!";

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                var state = await _stateProvider.GetAuthenticationStateAsync();
                var authenticationState = new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
                if (state.User.Identity.Name != new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity())).User.Identity.Name)
                {
                    _navigationManager.NavigateTo("/home");
                }
            }
        }

        private async Task SubmitAsync()
        {
            _tokenModel.Email = ReadOnlyEmail;
            _tokenModel.Password = ReadOnlyPWD;
            var result = await _authenticationManager.Login(_tokenModel);
            if (!result.Succeeded)
            {
                foreach (var message in result.Messages)
                {
                    _snackBar.Add(message, Severity.Error);
                }
            }
        }

        void TogglePasswordVisibility()
        {
            if (_passwordVisibility)
            {
                _passwordVisibility = false;
                _passwordInputIcon = Icons.Material.Filled.VisibilityOff;
                _passwordInput = InputType.Password;
            }
            else
            {
                _passwordVisibility = true;
                _passwordInputIcon = Icons.Material.Filled.Visibility;
                _passwordInput = InputType.Text;
            }
        }
    }
}