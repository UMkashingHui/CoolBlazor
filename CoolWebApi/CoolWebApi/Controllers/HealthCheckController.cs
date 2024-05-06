using Microsoft.AspNetCore.Mvc;

namespace CoolWebApi.Controllers;

[ApiController]
[Route("[controller]")]
public class HealthCheckController : ControllerBase
{
    private readonly ILogger<HealthCheckController> _logger;

    public HealthCheckController(ILogger<HealthCheckController> logger)
    {
        _logger = logger;
    }

    [HttpGet(Name = "HealthCheck")]
    public String Get()
    {
        return "the@health@is@good";
    }
}

