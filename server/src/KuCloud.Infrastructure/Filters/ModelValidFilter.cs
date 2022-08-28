using KuCloud.Infrastructure.Exceptions;

using Microsoft.AspNetCore.Mvc.Filters;

namespace KuCloud.Infrastructure.Filters;

public class ModelValidFilter : IAsyncResultFilter
{
    public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
    {
        if (!context.ModelState.IsValid)
        {
            var errors = context.ModelState
                .Where(x => x.Value?.Errors.Count > 0)
                .ToDictionary(pair => pair.Key, pair => pair.Value?.Errors.Select(x => x.ErrorMessage).ToArray());

            throw new ModelValidException(errors);
        }
        
        await next();
    }
}
