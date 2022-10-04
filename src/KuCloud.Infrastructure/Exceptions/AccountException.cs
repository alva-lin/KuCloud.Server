using KuCloud.Infrastructure.Common;
using KuCloud.Infrastructure.Enums;
using KuCloud.Infrastructure.Exceptions;

namespace KuCloud.Core.Exceptions;

public class AccountException : BasicException
{
    public AccountException(
        ErrorCode code,
        string? message = null,
        object? errorInfos = null,
        Exception? innerException = null)
        : base(code, message, errorInfos, innerException)
    {

    }
}
