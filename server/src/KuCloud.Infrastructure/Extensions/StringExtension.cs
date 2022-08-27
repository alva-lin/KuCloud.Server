namespace KuCloud.Infrastructure.Extensions;

public static class StringExtension
{
    public static bool IsNullOrWhiteSpace(this string str) => string.IsNullOrWhiteSpace(str);

    public static bool IsNullOrEmpty(this string str) => string.IsNullOrEmpty(str);

    public static string ToCamelCase(this string str)
    {
        if (!str.IsNullOrWhiteSpace() && str.Length > 1)
        {
            return str[0].ToString().ToLowerInvariant() + str.Substring(1);
        }
        return str.ToLowerInvariant();
    }
}
