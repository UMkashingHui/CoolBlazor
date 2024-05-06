using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CoolWebApi.Models.Identity;
using CoolWebApi.Services.Identity;

namespace CoolWebApi.Controllers.Identity
{
    [Area("Administrator")]
    [Authorize(Policy = "Administrator")]
    public class CurrentUserController : ControllerBase
    {
        // private readonly ApplicationDbContext _context;
        private readonly ILogger _logger;
        private readonly ICurrentUserService _service;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public CurrentUserController(ICurrentUserService service, ILogger<CurrentUserController> logger, IHttpContextAccessor httpContextAccessor)
        {
            _service = service;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            // _context = context;
            CurrentUser.Configure(_httpContextAccessor);
        }



    }
}