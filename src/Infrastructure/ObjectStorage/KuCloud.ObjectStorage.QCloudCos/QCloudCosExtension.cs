using KuCloud.ObjectStorage.Abstract;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace KuCloud.ObjectStorage.QCloudCos;

public static class QCloudCosExtension
{
    public static IServiceCollection AddQCloudCos(this IServiceCollection services, IConfiguration configuration, string? configPath = null)
    {
        configPath ??= nameof(QCloudCosOption);

        services.Configure<QCloudCosOption>(configuration.GetSection(configPath));

        services.AddScoped<IObjectStorageService, QCloudCosService>();
        services.AddScoped<QCloudCosService>();

        return services;
    }
}
