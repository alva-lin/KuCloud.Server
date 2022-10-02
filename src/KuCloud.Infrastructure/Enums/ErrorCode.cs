using System.ComponentModel;

namespace KuCloud.Infrastructure.Enums;

public enum ErrorCode
{
    [Description("success")]
    Success = 0,

    #region 系统内部错误

    [Description("unknown internal system error")]
    InternalSystemError = 10000,

    #endregion

    #region 服务请求错误

    [Description("unknown service error")]
    ServiceError = 20000,

    #endregion

    #region 服务请求失败

    [Description("unknown service fail")]
    ServiceFail = 30000,
    
    [Description("operation was canceled")]
    RequestCanceled = 30001,

    [Description("token not found")]
    TokenNotFound = 30101,

    [Description("token invalid")]
    TokenInvalid = 30102,

    [Description("token expired")]
    TokenExpired = 30103,

    [Description("model invalid")]
    ModelInvalid = 30201,

    [Description("entity not found")]
    EntityNotFound = 30301,

    [Description("account has been existed")]
    AccountHasBeenExisted = 30401,

    [Description("account or password error")]
    AccountOrPasswordError = 30402,

    #endregion

    #region 对象存储 40000

    [Description("object storage error")]
    ObjectStorageError = 40000,

    #endregion
}
