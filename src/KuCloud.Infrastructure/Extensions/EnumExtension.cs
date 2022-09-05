using System.ComponentModel;
using System.Reflection;

namespace KuCloud.Infrastructure.Extensions;

public static class EnumExtension
{
    public static string ToDescription(this Enum @enum)
    {
        var description = @enum.ToString("G");
        var field = @enum.GetType().GetField(@enum.ToString());
        var attr = field?.GetCustomAttributes<DescriptionAttribute>().FirstOrDefault();
        if (attr is not null)
        {
            description = attr.Description;
        }
        return description;
    }
}
