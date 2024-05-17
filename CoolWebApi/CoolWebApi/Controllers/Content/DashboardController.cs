using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using CoolWebApi.Services.Content;
using CoolWebApi.Utils.Constants.Permission;

namespace CoolWebApi.Controllers.Content
{
    [Authorize]
    [Route("api/dashboard")]
    [ApiController]
    public class DashboardController : ControllerBase
    {

        private readonly IDashBoardService _dashBoardService;

        public DashboardController(IDashBoardService dashBoardService)
        {
            _dashBoardService = dashBoardService;
        }
        /// <summary>
        /// Get Dashboard Data
        /// </summary>
        /// <returns>Status 200 OK </returns>
        [Authorize(Policy = Permissions.Dashboards.View)]
        [HttpGet]
        public async Task<IActionResult> GetDataAsync()
        {
            var result = await _dashBoardService.GetDashBoardData();
            // var result = await _mediator.Send(new GetDashboardDataQuery());
            return Ok(result);
        }
    }
}