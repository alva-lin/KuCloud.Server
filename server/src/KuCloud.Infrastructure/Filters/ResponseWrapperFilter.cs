using KuCloud.Infrastructure.Common;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace KuCloud.Infrastructure.Filters;

public class ResponseWrapperFilter : IAsyncResultFilter
{
    public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
    {
        if (context.Result is ObjectResult objectResult)
        {
            context.Result = new ObjectResult(ResponseModel<object?>.Success(objectResult.Value));
        }
        else if (context.Result is EmptyResult)
        {
            context.Result = new ObjectResult(ResponseModel<object?>.Success(VoidObject.Instance));
        }
        
        await next();
    }
}
