using KuCloud.Infrastructure.Common;
using KuCloud.Infrastructure.Enums;
using KuCloud.Infrastructure.Exceptions;
using KuCloud.Infrastructure.Extensions;

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
            var result = ResponseModel<object>.Error(e.Code, e.Message, e.ErrorInfos);
            await context.Response.WriteAsJsonAsync(result, cancellationToken);

            // TODO 错误代码的 CodeMean, Description 标签的内容
            _logger.LogError(e, "{Code} {Message} {Info}",
                e.Code,
                e.Message,
                e.ErrorInfos);
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("request canceled");
        }
        catch (Exception e)
        {
            var code = ErrorCode.ServiceError;
            var result = ResponseModel<object>.Error(code);
            await context.Response.WriteAsJsonAsync(result, cancellationToken);

            _logger.LogError(e,"{Code} {Message} {Info}",
                code,
                e.Message,
                e.StackTrace);
        }
    }
}
