using KuCloud.Data.Models;
using KuCloud.Dto.Account;
using KuCloud.Infrastructure.Attributes;
using KuCloud.Infrastructure.Enums;

namespace KuCloud.Services.Abstractions;

[LifeScope(LifeScope.Scope)]
public interface IAccountService : IBasicEntityService<Account, Guid>
{
    public Task Register(RegisterModel model, CancellationToken cancellationToken);
}
