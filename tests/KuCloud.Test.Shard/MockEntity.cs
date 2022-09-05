using KuCloud.Infrastructure.Entities;

namespace KuCloud.Test.Shard;

public class MockEntity : BasicEntity<int>
{
    public string StringProperty { get; set; } = string.Empty;

    public bool BooleanProperty { get; set; }

    public int IntegerProperty { get; set; }

    public DateTime DateTimeProperty { get; set; }
}
