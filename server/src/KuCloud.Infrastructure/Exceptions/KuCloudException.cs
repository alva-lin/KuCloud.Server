using KuCloud.Infrastructure.Enums;
using KuCloud.Infrastructure.Extensions;

namespace KuCloud.Infrastructure.Exceptions;

public class KuCloudException : Exception
{
    public KuCloudErrorCode Code { get; set; }

    public object? ErrorInfos { get; set; }

    // ReSharper disable once MemberCanBeProtected.Global
    public KuCloudException(
        KuCloudErrorCode code,
        string?          message        = null,
        object?          errorInfos     = null,
        Exception?       innerException = null)
        : base(message ?? code.ToDescription(), innerException)
    {
        Code       = code;
        ErrorInfos = errorInfos;
    }
}
