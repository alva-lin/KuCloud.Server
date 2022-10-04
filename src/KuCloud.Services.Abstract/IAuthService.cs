using KuCloud.Data.Dto.Account;
using KuCloud.Infrastructure.Attributes;
using KuCloud.Infrastructure.Common;
using KuCloud.Infrastructure.Enums;

namespace KuCloud.Services.Abstract;

[LifeScope(LifeScope.Scope)]
public interface IAuthService : IBasicService
{
    public Task<string> Login(LoginModel model, CancellationToken cancellationToken);
}
