using KuCloud.Infrastructure.Attributes;
using KuCloud.Infrastructure.Enums;
using KuCloud.Services.Abstractions;

using Microsoft.Extensions.DependencyInjection;

using System.Reflection;

namespace KuCloud.Core.Extensions;

public static class ServiceExtension
{
    /// <summary>
    ///     根据 <see cref="LifeScopeAttribute" /> 来注册服务
    /// </summary>
    public static IServiceCollection AddKuCloudServiceByLifeScope(this IServiceCollection services, params Assembly[] assemblys)
    {
        // TODO 遍历注册
        // var assemblys = new[] { implementAssembly };
        var types = assemblys.SelectMany(assembly => assembly.GetTypes())
            .Where(type => type.IsClass && !type.IsGenericType && !type.IsAbstract &&
                type.Name.EndsWith("Service") &&
                type.GetInterface(nameof(IKuCloudService)) != null)
            .ToList();

        foreach (var type in types)
        {
            var lifeScope = type.GetCustomAttribute<LifeScopeAttribute>()?.Scope ?? LifeScope.Transient;
            switch (lifeScope)
            {
                case LifeScope.Transient:
                    services.AddTransient(type);
                    break;
                case LifeScope.Scope:
                    services.AddScoped(type);
                    break;
                case LifeScope.Singleton:
                    services.AddSingleton(type);
                    break;
            }

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
}
