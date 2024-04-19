using Blazored.FluentValidation;
using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor;
using System.Security.Claims;
using System.Threading.Tasks;
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


        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                var state = await _stateProvider.GetAuthenticationStateAsync();
                // var authenticationState = new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
                if (state.User.Identity.Name != new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity())).User.Identity.Name)
                // if (state != new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity())))
                {
                    _navigationManager.NavigateTo("/home");
                }
                StateHasChanged();

            }
        }

        // protected override async Task OnInitializedAsync()
        // {
        //     var state = await _stateProvider.GetAuthenticationStateAsync();
        //     // var authenticationState = new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        //     if (state.User.Identity.Name != new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity())).User.Identity.Name)
        //     // if (state != new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity())))
        //     {
        //         _navigationManager.NavigateTo("/home");
        //     }
        // }

        private async Task SubmitAsync()
        {
            var result = await _authenticationManager.Login(_tokenModel);
            if (!result.Succeeded)
            {
                foreach (var message in result.Messages)
                {
                    _snackBar.Add(message, Severity.Error);
                }
            }
            // else
            // {
            //     _navigationManager.NavigateTo("/");
            // }
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

        // Original 

        public string Text { get; set; } = "admin123";
        public string Password { get; set; } = "admin123";
        bool success;
        bool isShow;
        string[] errors = { };
        MudTextField<string> pwField1;
        MudForm form;
        InputType PasswordInput = InputType.Password;
        InputType AccountInput = InputType.Text;

        string PasswordInputIcon = Icons.Material.Filled.VisibilityOff;

        void ButtonTestclick()
        {
            if (isShow)
            {
                isShow = false;
                PasswordInputIcon = Icons.Material.Filled.VisibilityOff;
                PasswordInput = InputType.Password;
            }
            else
            {
                isShow = true;
                PasswordInputIcon = Icons.Material.Filled.Visibility;
                PasswordInput = InputType.Text;
            }
        }

        private IEnumerable<string> PasswordStrength(string pw)
        {
            if (string.IsNullOrWhiteSpace(pw))
            {
                yield return "Password is required!";
                yield break;
            }
            if (pw.Length < 8)
                yield return "Password must be at least of length 8";
            if (!Regex.IsMatch(pw, @"[A-Z]"))
                yield return "Password must contain at least one capital letter";
            if (!Regex.IsMatch(pw, @"[a-z]"))
                yield return "Password must contain at least one lowercase letter";
            if (!Regex.IsMatch(pw, @"[0-9]"))
                yield return "Password must contain at least one digit";
        }

        private string PasswordMatch(string arg)
        {
            if (pwField1.Value != arg)
                return "Passwords don't match";
            return null;
        }

    }
}