using System.ComponentModel;

namespace KuCloud.Infrastructure.Enums;

public enum KuCloudErrorCode
{
    [Description("success")]
    Success = 0,
    
    #region 系统内部错误

    [Description("internal system error")]
    InternalSystemError = 10000,

    #endregion

    #region 服务请求错误

    [Description("service error")]
    ServiceError = 20000,

    #endregion

    #region 服务请求失败

    [Description("service fail")]
    ServiceFail = 30000,
    
    [Description("token not found")]
    TokenNotFound = 30001,
    
    [Description("token invalid")]
    TokenInvalid = 30002,
    
    [Description("token expired")]
    TokenExpired = 30003,
    
    [Description("model invalid")]
    ModelInvalid = 30101,
    
    [Description("entity not found")]
    EntityNotFound = 30201,

    #endregion
}
