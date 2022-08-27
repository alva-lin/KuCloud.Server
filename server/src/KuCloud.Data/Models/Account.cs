using KuCloud.Infrastructure.Entities;
using KuCloud.Infrastructure.Helpers;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Globalization;

namespace KuCloud.Data.Models;

/// <summary>
///     账户
/// </summary>
public class Account : BasicEntity<Guid>
{
    /// <summary>
    ///     账户名
    /// </summary>
    [Required]
    public string Name { get; set; } = null!;

    /// <summary>
    ///     邮箱
    /// </summary>
    [EmailAddress]
    public string? Email { get; set; }

    /// <summary>
    ///     手机号
    /// </summary>
    [Phone]
    public string? Phone { get; set; }

    /// <summary>
    ///     用户名
    /// </summary>
    [Required]
    public string DisplayName { get; set; } = null!;

    /// <summary>
    ///     密码
    /// </summary>
    [Required]
    public string Password { get; protected set; } = null!;

    /// <summary>
    ///     用于加密密码的盐
    /// </summary>
    [Required]
    public string Salt { get; set; } = null!;

    private string CalcPassword(string password) => CryptoHelper.Md5(password, Salt);

    public void SetPassword(string password) => Password = CalcPassword(password);

    public bool CheckPassword(string password) => Password == CalcPassword(password);

    public static Account GenerateAccount(string name, string unHashedPassword)
    {
        var account = new Account()
        {
            Name = name,
            DisplayName = name,
            Salt = CryptoHelper.Md5(name, DateTime.UtcNow.ToString(CultureInfo.InvariantCulture), new Random().Next(int.MaxValue).ToString())
        };
        account.SetPassword(unHashedPassword);
        return account;
    }
}

public class AccountTypeConfiguration : IEntityTypeConfiguration<Account>
{
    public void Configure(EntityTypeBuilder<Account> builder) { }
}
