namespace KuCloud.Infrastructure.Options;

/// <summary>
/// Jwt 配置
/// </summary>
public class JwtOption : IBasicOption
{
    /// <summary>
    /// 加密密钥
    /// </summary>
    public string Secret { get; set; } = null!;

    /// <summary>
    /// 颁发者
    /// </summary>
    public string Issuer { get; set; } = null!;

    /// <summary>
    /// 接收者
    /// </summary>
    public string Audience { get; set; } = null!;

    /// <summary>
    /// 过期时间
    /// </summary>
    public uint ExpiredSecond { get; set; }
}
