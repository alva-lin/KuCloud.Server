using KuCloud.Infrastructure.Enums;

namespace KuCloud.Infrastructure.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface)]
public class LifeScopeAttribute : Attribute
{
    public LifeScopeAttribute(LifeScope scope)
    {
        Scope = scope;
    }

    public LifeScope Scope { get; set; }
}
