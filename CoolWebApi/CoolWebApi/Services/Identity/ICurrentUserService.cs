using System;
using System.Security.Claims;
namespace CoolWebApi.Services.Identity
{
    public interface ICurrentUserService : IService
    {
        string UserId { get; }
    }
}

