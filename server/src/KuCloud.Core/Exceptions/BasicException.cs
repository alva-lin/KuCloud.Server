using KuCloud.Core.Enums;
using KuCloud.Infrastructure.Extensions;

namespace KuCloud.Core.Exceptions;

public class BasicException : Exception
{
    // ReSharper disable once MemberCanBeProtected.Global
    public BasicException(
        KuCloudErrorCode code,
        string? message = null,
        object? errorInfos = null,
        Exception? innerException = null)
        : base(message ?? code.ToDescription(), innerException)
    {
        Code = code;
        ErrorInfos = errorInfos;
    }

    public KuCloudErrorCode Code { get; set; }

    public object? ErrorInfos { get; set; }
}
