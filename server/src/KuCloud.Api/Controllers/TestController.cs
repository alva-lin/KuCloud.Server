using KuCloud.Infrastructure.Common;
using KuCloud.Services.Abstractions;
using KuCloud.Services.Abstractions.CommonServices;

using Microsoft.AspNetCore.Mvc;

namespace KuCloud.Api.Controllers;

/// <summary>
/// 测试控制器
/// </summary>
public class TestController : BasicController
{
    private readonly ILogger<TestController> _logger;

    private readonly IAccountService _accountService;

    private readonly IAuthService _authService;

    public TestController(ILogger<TestController> logger, IAuthService authService, IAccountService accountService)
    {
        _logger = logger;
        _authService = authService;
        _accountService = accountService;
    }

    /// <summary>
    /// Echo
    /// </summary>
    [HttpGet("[action]/{msg}")]
    public string Echo(string msg)
    {
        return msg;
    }
}
