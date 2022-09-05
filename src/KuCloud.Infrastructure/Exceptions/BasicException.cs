namespace KuCloud.Infrastructure.Exceptions;

public class BasicException : Exception
{
    // ReSharper disable once MemberCanBeProtected.Global
    public BasicException(
        int code,
        string? message = null,
        object? errorInfos = null,
        Exception? innerException = null)
        : base(message, innerException)
    {
        Code = code;
        ErrorInfos = errorInfos;
    }

    public int Code { get; set; }

    public object? ErrorInfos { get; set; }
}
