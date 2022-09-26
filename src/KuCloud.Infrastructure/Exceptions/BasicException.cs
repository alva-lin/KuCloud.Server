using KuCloud.Infrastructure.Common;
using KuCloud.Infrastructure.Enums;
using KuCloud.Infrastructure.Extensions;

namespace KuCloud.Infrastructure.Exceptions;

public class BasicException : Exception
{
    // ReSharper disable once MemberCanBeProtected.Global
    public BasicException(
        ErrorCode code,
        string? message = null,
        object? errorInfos = null,
        Exception? innerException = null)
        : base(message ?? code.ToDescription(), innerException)
    {
        Code = code;
        ErrorInfos = errorInfos;
    }

    public ErrorCode Code { get; set; }

    public object? ErrorInfos { get; set; }
}
