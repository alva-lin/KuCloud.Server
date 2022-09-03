namespace KuCloud.Infrastructure.Options;

/// <summary>
/// Cors 配置
/// </summary>
public class CorsOption : IBasicOption
{
    public string[] AllowOrigins { get; set; } = Array.Empty<string>();
}
