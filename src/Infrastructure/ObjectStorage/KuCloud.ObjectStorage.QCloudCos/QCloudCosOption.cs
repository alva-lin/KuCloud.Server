namespace KuCloud.ObjectStorage.QCloudCos;

/// <summary>
/// 腾讯云对象存储配置项
/// </summary>
public class QCloudCosOption
{
    /// <summary>
    /// 腾讯云账户的账户标识 APPID
    /// </summary>
    public string AppId { get; set; } = null!;

    /// <summary>
    /// 腾讯云的云 API 密钥 SecretId
    /// </summary>
    public string SecretId { get; set; } = null!;

    /// <summary>
    /// 腾讯云的云 API 密钥 SecretKey
    /// </summary>
    public string SecretKey { get; set; } = null!;

    /// <summary>
    /// 存储桶区域
    /// </summary>
    public string Region { get; set; } = null!;

    /// <summary>
    /// 存储桶名称
    /// </summary>
    public string BucketName { get; set; } = null!;

    /// <summary>
    /// BucketName-APPID
    /// </summary>
    public string Bucket => $"{BucketName}-{AppId}";

    /// <summary>
    /// 路径前缀
    /// </summary>
    public string BasePath { get; set; } = string.Empty;

    /// <summary>
    /// 每次请求签名有效时长，单位为秒
    /// </summary>
    public int DurationSecond { get; set; }
}
