using KuCloud.Infrastructure.Common;
using KuCloud.Infrastructure.Enums;
using KuCloud.Infrastructure.Exceptions;
using KuCloud.Infrastructure.Extensions;
using KuCloud.Infrastructure.Options;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace KuCloud.Api.Controllers;

/// <summary>
///     测试控制器
/// </summary>
public class TestController : BasicController
{
    private readonly ILogger<TestController> _logger;

    private readonly IServiceProvider _provider;

    public TestController(ILogger<TestController> logger, IServiceProvider provider)
    {
        _logger = logger;
        _provider = provider;
    }

    /// <summary>
    ///     Echo
    /// </summary>
    [HttpGet("[action]/{msg}")]
    public string Echo(string msg) => msg;

    /// <summary>
    /// 异常抛出测试接口
    /// </summary>
    [HttpGet("[action]")]
    public IEnumerable<int> ThrowBasicException(bool isBasic)
    {
        if (isBasic)
        {
            throw new BasicException(ErrorCode.ServiceFail);
        }
        throw new();
    }

    /// <summary>
    /// 获取配置项
    /// </summary>
    /// <param name="optionName"></param>
    /// <returns></returns>
    /// <exception cref="BasicException"></exception>
    [HttpGet("[action]")]
    public IActionResult GetOption(string optionName)
    {
        var types = ServiceExtension.AllNormalTypes.Where(type => type.GetInterface(nameof(IBasicOption)) is { }).ToArray();

        var type = types.FirstOrDefault(type1 => type1.Name.Equals(optionName, StringComparison.OrdinalIgnoreCase));
        if (type == null)
        {
            throw new BasicException(ErrorCode.ServiceFail, $"Options {optionName} not found");
        }

        using var scope = _provider.CreateScope();
        var services = scope.ServiceProvider;

        var genericType = typeof(IOptionsSnapshot<>).MakeGenericType(type);

        var option = services.GetService(genericType);
        if (option == null)
        {
            throw new BasicException(ErrorCode.ServiceFail, $"Options {optionName} not found");
        }

        var result = ((IOptionsSnapshot<IBasicOption>)option).Value;

        return Ok(result);
    }
}
