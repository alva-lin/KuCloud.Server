using KuCloud.Infrastructure.Attributes;
using KuCloud.Infrastructure.Common;
using KuCloud.Infrastructure.Enums;
using KuCloud.Infrastructure.Options;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

using System.Reflection;

// ReSharper disable UnusedMethodReturnValue.Global

namespace KuCloud.Infrastructure.Extensions;

public static class ServiceExtension
{
    /// <summary>
    /// 所有加载的程序集
    /// </summary>
    private static Assembly[] AllAssemblies => AppDomain.CurrentDomain.GetAssemblies().OrderBy(assembly => assembly.FullName).ToArray();

    private static Type[] AllTypes => AllAssemblies.SelectMany(assembly => assembly.GetTypes()).ToArray();

    public static Type[] AllNormalTypes => AllTypes.Where(type => type.IsClass && !type.IsGenericType && !type.IsAbstract).ToArray();

    /// <summary>
    ///     根据 <see cref="LifeScopeAttribute" /> 来注册服务
    /// </summary>
    public static IServiceCollection AddBasicServiceByLifeScope(this IServiceCollection services)
    {
        var types = AllNormalTypes
            .Where(type => type.GetInterface(nameof(IBasicService)) != null)
            .ToList();

        foreach (var type in types)
        {
            var lifeScope = type.GetCustomAttribute<LifeScopeAttribute>()?.Scope ?? LifeScope.Transient;

            var implements = type.GetInterfaces()
                .Where(iType => !iType.IsGenericType && iType.Name.EndsWith("Service"));
            foreach (var implement in implements)
            {
                var iScope = implement.GetCustomAttribute<LifeScopeAttribute>()?.Scope ?? lifeScope;

                switch (iScope)
                {
                    case LifeScope.Transient:
                        services.AddTransient(implement, type);
                        break;
                    case LifeScope.Scope:
                        services.AddScoped(implement, type);
                        break;
                    case LifeScope.Singleton:
                        services.AddSingleton(implement, type);
                        break;
                }
            }
        }

        return services;
    }

    public static IServiceCollection AddBasicOptions(this IServiceCollection services, IConfiguration configuration)
    {
        var types = AllNormalTypes
            .Where(type =>
                type.Name.EndsWith("Option") &&
                type.GetInterface(nameof(IBasicOption)) != null)
            .ToList();

        var method = typeof(OptionsConfigurationServiceCollectionExtensions).GetMethod("Configure", new[] { typeof(IServiceCollection), typeof(IConfiguration) })!;
        foreach (var type in types)
        {
            var genericMethod = method.MakeGenericMethod(type);
            genericMethod.Invoke(null, new object[] { services, configuration.GetSection(type.Name) });
        }

        return services;
    }

    public static IServiceCollection AddJwtBearer(this IServiceCollection services)
    {
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer();

        services.ConfigureOptions<ConfigureJwtBearerOptions>();

        return services;
    }

    public static IServiceCollection AddCorsSetting(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            var serviceProvider = services.BuildServiceProvider();
            var corsOption      = serviceProvider!.GetRequiredService<IOptions<CorsOption>>().Value;

            options.AddPolicy("Develop", builder =>
            {
                builder.WithOrigins(corsOption.AllowOrigins);
            });
        });

        return services;
    }
}
