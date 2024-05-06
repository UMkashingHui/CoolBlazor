using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using CoolWebApi.Models.Identity;

namespace CoolWebApi.Services.Identity.impl
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            // UserName = _httpContextAccessor.HttpContext.User.Identity.Name;
            UserId = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            // User = _httpContextAccessor.HttpContext.User;
            // Claims = _httpContextAccessor.HttpContext?.User?.Claims.AsEnumerable().Select(item => new KeyValuePair<string, string>(item.Type, item.Value)).ToList();
        }
        // public string UserName { get; }
        public string UserId { get; }
        // public ClaimsPrincipal User { get; }
        // public List<KeyValuePair<string, string>> Claims { get; set; }
    }


    // public class CurrentUserService : ICurrentUserService
    // {

    //     private IHttpContextAccessor _httpContextAccessor;

    //     public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    //     {
    //         _httpContextAccessor = httpContextAccessor;
    //     }

    //     // public string UserId { get { return _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier).Value; } }
    //     // public string UserId => _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);

    //     // string ICurrentUserService.UserId { get; set; }
    // }


    // public class CurrentUserService : ICurrentUserService
    // {
    //     public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    //     {
    //         UserId = httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
    //         Claims = httpContextAccessor.HttpContext?.User?.Claims.AsEnumerable().Select(item => new KeyValuePair<string, string>(item.Type, item.Value)).ToList();
    //     }

    //     public string UserId { get; }
    //     public List<KeyValuePair<string, string>> Claims { get; set; }
    // }

}

