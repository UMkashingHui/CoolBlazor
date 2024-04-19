
using System.Threading.Tasks;
using CoolBlazor.Infrastructure.Managers;
using CoolBlazor.Infrastructure.Models.Requests.Identity;
using CoolBlazor.Infrastructure.Utils.Wrapper;
using IResult = CoolBlazor.Infrastructure.Utils.Wrapper.IResult;

namespace CoolBlazor.Infrastructure.Managers.Identity.Account
{
    public interface IAccountManager : IManager
    {
        Task<IResult> ChangePasswordAsync(ChangePasswordRequest model);

        Task<IResult> UpdateProfileAsync(UpdateProfileRequest model);

        Task<IResult<string>> GetProfilePictureAsync(string userId);

        Task<IResult<string>> UpdateProfilePictureAsync(UpdateProfilePictureRequest request, string userId);
    }
}