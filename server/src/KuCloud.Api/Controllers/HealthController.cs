using Microsoft.AspNetCore.Mvc;

namespace KuCloud.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class HealthController : ControllerBase
{
    private readonly ILogger<HealthController> _logger;

    public HealthController(ILogger<HealthController> logger)
    {
        _logger = logger;
    }

    [HttpGet("[action]")]
    public IActionResult Heartbeat()
    {
        return Ok();
    }
}
