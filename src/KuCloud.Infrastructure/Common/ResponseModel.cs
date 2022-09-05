﻿namespace KuCloud.Infrastructure.Common;

/// <summary>
/// 响应结果包装器
/// </summary>
/// <typeparam name="TModel"></typeparam>
public class ResponseModel<TModel>
{
    /// <summary>
    /// 响应码
    /// </summary>
    public int Code { get; set; }

    /// <summary>
    /// 响应信息
    /// </summary>
    public string? Message { get; set; }

    /// <summary>
    /// 响应数据
    /// </summary>
    public TModel? Data { get; set; }

    public static ResponseModel<TModel> Success(TModel data, string? message = null) =>
        new()
        {
            Data = data,
            Code = ResponseCode.Success,
            Message = message
        };

    public static ResponseModel<TModel> Fail(int code, string? message = null, TModel? data = default) =>
        new()
        {
            Data = data,
            Code = code,
            Message = message
        };

    public static ResponseModel<TModel> Error(int code, string? message = null, TModel? data = default) =>
        new()
        {
            Data = data,
            Code = code,
            Message = message
        };
}