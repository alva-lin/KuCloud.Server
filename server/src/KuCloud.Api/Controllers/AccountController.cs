using KuCloud.Dto.Account;
using KuCloud.Infrastructure.Common;
using KuCloud.Services;

using Microsoft.AspNetCore.Mvc;

namespace KuCloud.Api.Controllers;

/// <summary>
/// 账户相关接口
/// </summary>
public class AccountController : BasicController
{
    private readonly AccountService _accountService;

    public AccountController(AccountService accountService)
    {
        _accountService = accountService;
    }

    /// <summary>
    ///     注册
    /// </summary>
    /// <param name="model">注册模型</param>
    /// <param name="cancellationToken"></param>
    [HttpPost("[action]")]
    public async Task Register(RegisterModel model, CancellationToken cancellationToken) =>
        await _accountService.Register(model, cancellationToken);
}
