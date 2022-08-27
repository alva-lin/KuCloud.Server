using KuCloud.Core.Enums;

namespace KuCloud.Core.Exceptions;

public class AccountException : BasicException
{
    public AccountException(
        KuCloudErrorCode code,
        string? message = null,
        object? errorInfos = null,
        Exception? innerException = null)
        : base(code, message, errorInfos, innerException)
    {

    }
}
