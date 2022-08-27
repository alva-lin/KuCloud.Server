using System.ComponentModel.DataAnnotations;

namespace KuCloud.Dto.Account;

public class RegisterModel
{
    [Required]
    public string Name { get; set; } = null!;

    [Required]
    public string Password { get; set; } = null!;
}

public class LoginModel
{
    [Required]
    public string Name { get; set; } = null!;

    [Required]
    public string Password { get; set; } = null!;
}
