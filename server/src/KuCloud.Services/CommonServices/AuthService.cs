using KuCloud.Core.Exceptions;
using KuCloud.Dto.Account;
using KuCloud.Infrastructure.Attributes;
using KuCloud.Infrastructure.Common;
using KuCloud.Infrastructure.Enums;
using KuCloud.Infrastructure.Options;
using KuCloud.Services.Abstractions;
using KuCloud.Services.Abstractions.CommonServices;

using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace KuCloud.Services;

[LifeScope(LifeScope.Scope)]
public class AuthService : IAuthService
{
    private readonly IAccountService _accountService;

    private readonly JwtOption _jwtOption;

    public AuthService(IAccountService accountService, IOptionsMonitor<JwtOption> jwtOption)
    {
        _accountService = accountService;
        _jwtOption = jwtOption.CurrentValue;
    }

    public async Task<string> Login(LoginModel model, CancellationToken cancellationToken = default)
    {
        var account = await _accountService.FindAsync(entity => entity.Name == model.Name, cancellationToken);
        if (account == null)
        {
            throw new AccountException(ResponseCode.AccountOrPasswordError);
        }

        if (!account.CheckPassword(model.Password))
        {
            throw new AccountException(ResponseCode.AccountOrPasswordError);
        }

        return GenerateToken();
    }

    private string GenerateToken()
    {
        var userName = "Alva";
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, userName),
            new Claim(JwtRegisteredClaimNames.Exp, $"{new DateTimeOffset(DateTime.Now.AddMinutes(30)).ToUnixTimeSeconds()}"),
            new Claim(JwtRegisteredClaimNames.Nbf, $"{new DateTimeOffset(DateTime.Now).ToUnixTimeSeconds()}"),
        };
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOption.Secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            issuer: _jwtOption.Issuer,
            audience: _jwtOption.Audience,
            claims: claims,
            expires: DateTime.Now.AddSeconds(_jwtOption.ExpiredSecond),
            signingCredentials: creds);

        var tokenStr = new JwtSecurityTokenHandler().WriteToken(token);
        return tokenStr;
    }
}
