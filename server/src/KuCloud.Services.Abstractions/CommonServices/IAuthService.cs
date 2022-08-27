using KuCloud.Dto.Account;
using KuCloud.Infrastructure.Attributes;
using KuCloud.Infrastructure.Enums;

namespace KuCloud.Services.Abstractions.CommonServices;

[LifeScope(LifeScope.Scope)]
public interface IAuthService : IKuCloudService
{
    public Task<string> Login(LoginModel model, CancellationToken cancellationToken);
}
