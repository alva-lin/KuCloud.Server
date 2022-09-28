using KuCloud.ObjectStorage.Abstract;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace KuCloud.ObjectStorage.QCloudCos;

public static class TencentCosExtension
{
    public static IServiceCollection AddTencentCos(this IServiceCollection services, IConfiguration configuration, string? configPath = null)
    {
        configPath ??= nameof(TencentCosOption);

        services.Configure<TencentCosOption>(configuration.GetSection(configPath));

        services.AddScoped<IObjectStorageService, TencentCloudObjectStorageService>();
        services.AddScoped<TencentCloudObjectStorageService>();

        return services;
    }
}
