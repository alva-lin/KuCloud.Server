using KuCloud.Infrastructure.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using System.ComponentModel.DataAnnotations;

namespace KuCloud.Data.Models;

/// <summary>
/// 账户
/// </summary>
public class Account : DeletableEntity<Guid>
{
    /// <summary>
    /// 用户名
    /// </summary>
    [Required]
    public string UserName { get; set; } = null!;

    /// <summary>
    /// 邮箱
    /// </summary>
    [EmailAddress]
    public string? Email { get; set; }

    /// <summary>
    /// 手机号
    /// </summary>
    [Phone]
    public string? Phone { get; set; }

    /// <summary>
    /// 展示名
    /// </summary>
    [Required]
    public string DisplayName { get; set; } = null!;

    /// <summary>
    /// 密码
    /// </summary>
    [Required]
    public string Password { get; set; } = null!;

    /// <summary>
    /// 用于加密密码的盐
    /// </summary>
    [Required]
    public string Salt { get; set; } = null!;
}


public class AccountTypeConfiguration : IEntityTypeConfiguration<Account>
{
    public void Configure(EntityTypeBuilder<Account> builder)
    {
        builder.HasQueryFilter(account => !account.IsDelete);
    }
}
