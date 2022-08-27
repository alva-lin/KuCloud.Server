using KuCloud.Core.Enums;
using KuCloud.Core.Exceptions;
using KuCloud.Dto.Account;
using KuCloud.Infrastructure.Attributes;
using KuCloud.Infrastructure.Enums;
using KuCloud.Infrastructure.Helpers;
using KuCloud.Services.Abstractions;
using KuCloud.Services.Abstractions.CommonServices;

namespace KuCloud.Services;

[LifeScope(LifeScope.Scope)]
public class AuthService : IAuthService
{
    private readonly IAccountService _accountService;

    public AuthService(IAccountService accountService)
    {
        _accountService = accountService;
    }

    public async Task<string> Login(LoginModel model, CancellationToken cancellationToken = default)
    {
        var account = await _accountService.FindAsync(entity => entity.Name == model.Name, cancellationToken);
        if (account == null)
        {
            throw new AccountException(KuCloudErrorCode.AccountOrPasswordError);
        }

        if (!account.CheckPassword(model.Password))
        {
            throw new AccountException(KuCloudErrorCode.AccountOrPasswordError);
        }

        return "Login Success";
    }
}
