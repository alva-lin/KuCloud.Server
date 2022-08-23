using KuCloud.Data;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = Host.CreateDefaultBuilder(args);

builder.ConfigureServices((context, services) =>
{
    var configuration = context.Configuration;
    var assemblyName = typeof(Program).Assembly.GetName().Name ?? string.Empty;
    services.AddDbContext<KuCloudDbContext>(optionsBuilder =>
        optionsBuilder.UseNpgsql(configuration.GetConnectionString("KuCloud"), contextOptionsBuilder => contextOptionsBuilder.MigrationsAssembly(assemblyName)));
});

var host = builder.Build();

Console.WriteLine("开始数据库迁移");
using (var scope = host.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<KuCloudDbContext>();
    db.Database.Migrate();
}
Console.WriteLine("数据库迁移结束，请检查数据库变更是否生效");
