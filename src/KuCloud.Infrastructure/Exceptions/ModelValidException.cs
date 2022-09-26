using KuCloud.Infrastructure.Common;
using KuCloud.Infrastructure.Enums;

namespace KuCloud.Infrastructure.Exceptions;

public class ModelValidException : BasicException
{
    public ModelValidException(Dictionary<string, string[]?> errorInfo)
        : base(ErrorCode.ModelInvalid,
            "输入参数不合法",
            errorInfo)
    {
    }
}
