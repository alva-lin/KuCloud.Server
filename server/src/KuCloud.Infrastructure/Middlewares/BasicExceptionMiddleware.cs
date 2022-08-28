using KuCloud.Infrastructure.Common;
using KuCloud.Infrastructure.Exceptions;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

using System.Text.Json;

namespace KuCloud.Infrastructure.Middlewares;

public class BasicExceptionMiddleware
{
    private readonly ILogger<BasicExceptionMiddleware> _logger;
    private readonly RequestDelegate _next;

    public BasicExceptionMiddleware(RequestDelegate next, ILogger<BasicExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var cancellationToken = context.RequestAborted;
        try
        {
            await _next(context);
        }
        catch (BasicException e)
        {
            // TODO Json 序列化相关内容
            var result = ResponseModel<object>.Error(e.Code, e.Message, e.ErrorInfos);
            var json = JsonSerializer.Serialize(result);
            await context.Response.WriteAsync(json, cancellationToken);

            // TODO 错误代码的 CodeMean, Description 标签的内容
            _logger.LogError("{Code} {Message} {Info}",
                e.Code,
                e.Message,
                e.ErrorInfos);
        }
        catch (Exception e)
        {
            var code = ResponseCode.ServiceError;
            var result = ResponseModel<object>.Error(code, e.Message);
            var json = JsonSerializer.Serialize(result);
            await context.Response.WriteAsync(json, cancellationToken);

            _logger.LogError("{Code} {Message} {Info}",
                code,
                e.Message,
                e.StackTrace);
        }
    }
}
