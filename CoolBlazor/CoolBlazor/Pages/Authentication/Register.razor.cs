using Blazored.FluentValidation;
using MudBlazor;
using CoolBlazor.Infrastructure.Models.Requests.Identity;

namespace CoolBlazor.Pages.Authentication
{
    public partial class Register
    {
        private RegisterRequest _registerUserModel = new();
        private FluentValidationValidator _fluentValidationValidator;
        private bool Validated => _fluentValidationValidator.Validate(options => { options.IncludeAllRuleSets(); });
        MudTextField<string> pwField1;

        private async Task CreateAsync()
        {
            _snackBar.Add("Demo site can not register user.", Severity.Error);
            // var response = await _userManager.RegisterUserAsync(_registerUserModel);
            // if (response.Succeeded)
            // {
            //     _snackBar.Add(response.Messages[0], Severity.Success);
            //     _navigationManager.NavigateTo("/login");
            //     _registerUserModel = new RegisterRequest();
            // }
            // else
            // {
            //     foreach (var message in response.Messages)
            //     {
            //         _snackBar.Add(message, Severity.Error);
            //     }
            // }
        }

        private bool _passwordVisibility;
        private InputType _passwordInput = InputType.Password;
        private string _passwordInputIcon = Icons.Material.Filled.VisibilityOff;

        private void TogglePasswordVisibility()
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