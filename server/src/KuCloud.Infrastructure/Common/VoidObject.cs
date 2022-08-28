namespace KuCloud.Infrastructure.Common;

public sealed class VoidObject
{
    public static VoidObject Instance = new();

    private VoidObject() {}
}
