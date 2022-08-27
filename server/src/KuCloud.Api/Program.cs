using KuCloud.Core.Extensions;
using KuCloud.Data;
using KuCloud.Data.Models;
using KuCloud.Infrastructure.Entities;
using KuCloud.Services;

using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

using System.Reflection;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo()
    {
        Title = "Ku Cloud Api",
        Version = "v1",
        Description = "Ku Cloud 服务接口"
    });
    
    var xmlFile =  $"{typeof(Program).Assembly.GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);
});

builder.Services.AddKuCloudServiceByLifeScope(Assembly.GetAssembly(typeof(BasicEntityService<BasicEntity<int>, int>))!);

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

app.UseAuthorization();

app.MapControllers();

app.Run();
