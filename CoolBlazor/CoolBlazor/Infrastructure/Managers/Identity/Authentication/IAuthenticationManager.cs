using CoolBlazor.Infrastructure.Models.Requests.Identity;
using System.Security.Claims;
using System.Threading.Tasks;
using IResult = CoolBlazor.Infrastructure.Utils.Wrapper.IResult;

namespace CoolBlazor.Infrastructure.Managers.Identity.Authentication
{
    public interface IAuthenticationManager
    {
        Task<IResult> Login(TokenRequest _tokenRequest);

        Task<IResult> Logout();

        Task<string> RefreshToken();

        Task<string> TryRefreshToken();

        Task<string> TryForceRefreshToken();

        Task<ClaimsPrincipal> CurrentUser();
    }
}