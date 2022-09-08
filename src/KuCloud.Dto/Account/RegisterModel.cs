using System.ComponentModel.DataAnnotations;

namespace KuCloud.Dto.Account;

/// <summary>
///     注册账户的数据模型
/// </summary>
public class RegisterModel
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
