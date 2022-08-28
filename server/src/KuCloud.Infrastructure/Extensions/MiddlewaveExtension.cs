using KuCloud.Infrastructure.Middlewares;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;

namespace KuCloud.Infrastructure.Extensions;

public static class MiddlewaveExtension
{
    public static IApplicationBuilder UseBasicException(this IApplicationBuilder host)
    {
        host.UseMiddleware<BasicExceptionMiddleware>();

        return host;
    }
}
