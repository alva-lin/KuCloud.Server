using KuCloud.Dto.Account;
using KuCloud.Infrastructure.Common;
using KuCloud.Services.Abstractions;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KuCloud.Api.Controllers;

/// <summary>
/// 身份验证、授权相关接口
/// </summary>
public class AuthController : BasicController
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    ///     登录
    /// </summary>
    /// <param name="model">登录模型</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [AllowAnonymous]
    [HttpPost("[action]")]
    public Task<string> Login(LoginModel model, CancellationToken cancellationToken) =>
        _authService.Login(model, cancellationToken);
}
