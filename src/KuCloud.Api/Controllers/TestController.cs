using KuCloud.Infrastructure.Common;
using KuCloud.Infrastructure.Exceptions;
using KuCloud.Infrastructure.Options;
using KuCloud.Services.Abstractions;
using KuCloud.Services.Abstractions.CommonServices;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace KuCloud.Api.Controllers;

/// <summary>
///     测试控制器
/// </summary>
public class TestController : BasicController
{
    private readonly IAccountService _accountService;

    private readonly IAuthService _authService;
    private readonly ILogger<TestController> _logger;

    private readonly JwtOption _jwtOption;

    public TestController(ILogger<TestController> logger, IAuthService authService, IAccountService accountService, IOptions<JwtOption> jwtOption)
    {
        _logger = logger;
        _authService = authService;
        _accountService = accountService;
        _jwtOption = jwtOption.Value;

#if DEBUG
        ;
#else
        throw new BasicException(ResponseCode.ServiceFail, "无法请求测试环境专属接口");
#endif
    }

    /// <summary>
    ///     Echo
    /// </summary>
    [HttpGet("[action]/{msg}")]
    public string Echo(string msg) => msg;

    /// <summary>
    /// 异常抛出测试接口
    /// </summary>
    [HttpGet("[action]")]
    public IEnumerable<int> ThrowBasicException(bool isBasic)
    {
        if (isBasic)
        {
            throw new BasicException(ResponseCode.ServiceFail);
        }
        throw new();
    }
}
