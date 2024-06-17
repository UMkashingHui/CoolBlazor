using Microsoft.AspNetCore.Components;
using MudBlazor;
using System.Threading.Tasks;
using Blazored.FluentValidation;
using CoolBlazor.Infrastructure.Models.Requests.Identity;

namespace CoolBlazor.Pages.Identity
{
    public partial class RegisterUserModal
    {
        private FluentValidationValidator? _fluentValidationValidator;
        private bool Validated => _fluentValidationValidator.Validate(options => { options.IncludeAllRuleSets(); });
        private readonly RegisterRequest _registerUserModel = new();
        [CascadingParameter] private MudDialogInstance MudDialog { get; set; }

        private void Cancel()
        {
            MudDialog.Cancel();
        }

        private async Task SubmitAsync()
        {
            _snackBar.Add("Demo site can not register user.", Severity.Error);
            // var response = await _userManager.RegisterUserAsync(_registerUserModel);
            // if (response.Succeeded)
            // {
            //     _snackBar.Add(response.Messages[0], Severity.Success);
            //     MudDialog.Close();
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