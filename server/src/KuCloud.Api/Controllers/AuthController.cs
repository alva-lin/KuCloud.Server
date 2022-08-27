﻿using KuCloud.Dto.Account;
using KuCloud.Infrastructure.Common;
using KuCloud.Services.Abstractions.CommonServices;

using Microsoft.AspNetCore.Mvc;

namespace KuCloud.Api.Controllers;

public class AuthController : BasicController
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// 登录
    /// </summary>
    /// <param name="model">登录模型</param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost("[action]")]
    public Task<string> Login(LoginModel model, CancellationToken cancellationToken)
    {
        return _authService.Login(model, cancellationToken);
    }
}
