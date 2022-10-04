using KuCloud.Data.Dto.Account;
using KuCloud.Data.Models;
using KuCloud.Infrastructure.Attributes;
using KuCloud.Infrastructure.Common;
using KuCloud.Infrastructure.Enums;

namespace KuCloud.Services.Abstract;

[LifeScope(LifeScope.Scope)]
public interface IAccountService : IBasicService
{
    public Task Register(RegisterModel model, CancellationToken cancellationToken);
}
