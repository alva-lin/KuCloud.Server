using KuCloud.Data;
using KuCloud.Infrastructure.Extensions;
using KuCloud.Infrastructure.Options;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

using System.Reflection;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
// Add services to the container.

builder.Services.AddControllers(options =>
{
    options.Filters.AddResponseWrapperFilter().AddModelValidFilter();
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new()
    {
        Title = "Ku Cloud Api",
        Version = "v1",
        Description = "Ku Cloud 服务接口"
    });

    var basePath = Directory.GetParent(Environment.CurrentDirectory);
    if (basePath is { Exists: true })
    {
        var currentAssembly = Assembly.GetExecutingAssembly();
        var xmlDocs = currentAssembly.GetReferencedAssemblies()
            .Union(new[] { currentAssembly.GetName() })
            .Select(a => Path.Combine(basePath.FullName, a.Name!, $"{a.Name}.xml"))
            .Where(File.Exists)
            .ToArray();
        Array.ForEach(xmlDocs, s => options.IncludeXmlComments(s));
    }
});
builder.Services.AddKuCloudServiceByLifeScope();
builder.Services.AddBasicOptions(configuration);
builder.Services.AddJwtBearer();

builder.Services.AddDbContext<KuCloudDbContext>(optionsBuilder =>
    optionsBuilder.UseNpgsql(configuration.GetConnectionString("KuCloud")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseBasicException();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
