using System.ComponentModel.DataAnnotations;

namespace KuCloud.Data.Dto.Account;

/// <summary>
///     登录账户的数据模型
/// </summary>
public class LoginModel
{
    /// <summary>
    ///     账户名
    /// </summary>
    [Required]
    public string Account { get; set; } = null!;

    /// <summary>
    ///     账户密码
    /// </summary>
    [Required]
    public string Password { get; set; } = null!;
}
