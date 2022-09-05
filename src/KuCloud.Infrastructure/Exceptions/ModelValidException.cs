using KuCloud.Infrastructure.Common;

namespace KuCloud.Infrastructure.Exceptions;

public class ModelValidException : BasicException
{
    public ModelValidException(Dictionary<string, string[]?> errorInfo)
        : base(ResponseCode.ModelInvalid,
            "输入参数不合法",
            errorInfo)
    {
    }
}
