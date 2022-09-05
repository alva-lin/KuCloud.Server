using KuCloud.Infrastructure.Common;

using Microsoft.AspNetCore.Mvc;

namespace KuCloud.Api.Controllers;

/// <summary>
///     服务健康检测控制器
/// </summary>
public class HealthController : BasicController
{
    private readonly ILogger<HealthController> _logger;

    public HealthController(ILogger<HealthController> logger)
    {
        _logger = logger;
    }

    /// <summary>
    ///     心跳检测
    /// </summary>
    /// <returns></returns>
    [HttpGet("[action]")]
    public IActionResult Heartbeat() => Ok();
}
