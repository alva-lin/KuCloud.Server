using System.Diagnostics.CodeAnalysis;
using System.Web;

namespace KuCloud.Infrastructure.Extensions;

public static class StringExtension
{
    public static bool IsNullOrWhiteSpace([NotNullWhen(false)] this string? str) => string.IsNullOrWhiteSpace(str);

    public static bool IsNotNullOrWhiteSpace([NotNullWhen(true)] this string? str) => !string.IsNullOrWhiteSpace(str);

    public static bool IsNullOrEmpty([NotNullWhen(false)] this string? str) => string.IsNullOrEmpty(str);

    public static bool IsNotNullOrEmpty([NotNullWhen(true)] this string? str) => !string.IsNullOrEmpty(str);

    public static string ToCamelCase(this string str)
    {
        if (!str.IsNullOrWhiteSpace() && str.Length > 1)
        {
            return str[0].ToString().ToLowerInvariant() + str.Substring(1);
        }
        return str.ToLowerInvariant();
    }

    public static string UrlEncode(this string str, System.Text.Encoding? e  = null)
    {
        if (e == null)
        {
            return HttpUtility.UrlEncode(str);
        }
        return HttpUtility.UrlEncode(str, e);
    }
    
    public static string UrlDecode(this string str, System.Text.Encoding? e  = null)
    {
        if (e == null)
        {
            return HttpUtility.UrlDecode(str);
        }
        return HttpUtility.UrlDecode(str, e);
    }
}
