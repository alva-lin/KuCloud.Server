using System.ComponentModel;

namespace KuCloud.Infrastructure.Common;

public static class ResponseCode
{
    [Description("Success")]
    public static readonly int Success = 0;

    #region 系统内部错误

    [Description("internal system error")]
    public static readonly int InternalSystemError = 10000;

    #endregion

    #region 服务请求错误

    [Description("service error")]
    public static readonly int ServiceError = 20000;

    #endregion

    #region 服务请求失败

    [Description("service fail")]
    public static readonly int ServiceFail = 30000;

    [Description("token not found")]
    public static readonly int TokenNotFound = 30001;

    [Description("token invalid")]
    public static readonly int TokenInvalid = 30002;

    [Description("token expired")]
    public static readonly int TokenExpired = 30003;

    [Description("model invalid")]
    public static readonly int ModelInvalid = 30101;

    [Description("entity not found")]
    public static readonly int EntityNotFound = 30201;

    [Description("account has been existed")]
    public static readonly int AccountHasBeenExisted = 30301;

    [Description("account or password error")]
    public static readonly int AccountOrPasswordError = 30302;

    #endregion
}
