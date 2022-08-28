using KuCloud.Core.Exceptions;
using KuCloud.Data;
using KuCloud.Data.Models;
using KuCloud.Dto.Account;
using KuCloud.Infrastructure.Attributes;
using KuCloud.Infrastructure.Common;
using KuCloud.Infrastructure.Enums;
using KuCloud.Infrastructure.Helpers;
using KuCloud.Services.Abstractions;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using System.Globalization;

using Random = System.Random;

namespace KuCloud.Services;

[LifeScope(LifeScope.Scope)]
public class AccountService : BasicEntityService<Account, Guid>, IAccountService
{
    private readonly ILogger<AccountService> _logger;

    public AccountService(KuCloudDbContext dbContext, ILogger<AccountService> logger)
        : base(dbContext)
    {
        _logger = logger;
    }

    public async Task Register(RegisterModel model, CancellationToken cancellationToken = default)
    {
        var account = await DbSet.FirstOrDefaultAsync(entity => entity.Name == model.Name, cancellationToken);
        if (account is not null)
        {
            throw new AccountException(ResponseCode.AccountHasBeenExisted);
        }
        
        account = Account.GenerateAccount(model.Name, model.Password);
        await AddAsync(account, cancellationToken);
        _logger.LogInformation("a new account register: {Name}", account.Name);
    }
}
