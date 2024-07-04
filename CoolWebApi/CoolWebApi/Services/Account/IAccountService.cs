using System.Threading.Tasks;
using CoolWebApi.Models.Requests.Identity;
using CoolWebApi.Utils.Wrapper;
using IResult = CoolWebApi.Utils.Wrapper.IResult;

namespace CoolWebApi.Services.Account
{
    public interface IAccountService : IService
    {
        Task<IResult> UpdateProfileAsync(UpdateProfileRequest model, string userId);

        Task<IResult> ChangePasswordAsync(ChangePasswordRequest model, string userId);

        Task<IResult<string>> GetProfilePictureAsync(string userId);

        Task<IResult<string>> UpdateProfilePictureAsync(UpdateProfilePictureRequest request);
    }
}