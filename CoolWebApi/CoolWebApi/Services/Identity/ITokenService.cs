using System;
using CoolWebApi.Models.Requests.Identity;
using CoolWebApi.Models.Responses.Identity;
using CoolWebApi.Utils.Wrapper;

namespace CoolWebApi.Services.Identity
{
    public interface ITokenService : IService
    {
        Task<Result<TokenResponse>> LoginAsync(TokenRequest model);

        Task<Result<TokenResponse>> GetRefreshTokenAsync(RefreshTokenRequest model);
    }
}

