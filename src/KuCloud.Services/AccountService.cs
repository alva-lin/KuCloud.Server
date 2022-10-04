using KuCloud.Core.Exceptions;
using KuCloud.Data;
using KuCloud.Data.Dto.Account;
using KuCloud.Data.Models;
using KuCloud.Infrastructure.Enums;
using KuCloud.Services.Abstract;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace KuCloud.Services;

public class AccountService : IAccountService
{
    private readonly ILogger<AccountService> _logger;

    private readonly KuCloudDbContext _dbContext;

    private readonly DbSet<Account> _dbSet;

    public AccountService(KuCloudDbContext dbContext, ILogger<AccountService> logger)
    {
        _dbContext = dbContext;
        _dbSet     = _dbContext.Accounts;
        _logger    = logger;
    }

    public async Task Register(RegisterModel model, CancellationToken cancellationToken = default)
    {
        var account = await _dbSet.FirstOrDefaultAsync(entity => entity.Name == model.Account, cancellationToken);
        if (account is not null)
        {
            throw new AccountException(ErrorCode.AccountHasBeenExisted);
        }

        account = Account.GenerateAccount(model.Account, model.Password);

        await _dbSet.AddAsync(account, cancellationToken);

        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("a new account register: {Name}", account.Name);
    }
}
