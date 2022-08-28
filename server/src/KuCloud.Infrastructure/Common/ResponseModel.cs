namespace KuCloud.Infrastructure.Common;

public class ResponseModel<TModel>
{
    public TModel? Data { get; set; }

    public int Code { get; set; }

    public string? Message { get; set; }

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
