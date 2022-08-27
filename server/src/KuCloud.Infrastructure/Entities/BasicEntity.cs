namespace KuCloud.Infrastructure.Entities;

public abstract class BasicEntity<TKey> : IBasicEntity<TKey> where TKey : IEquatable<TKey>
{
    public TKey Id { get; set; } = default!;

    public string CreatedBy { get; set; } = string.Empty;

    public DateTime CreatedTime { get; set; }

    public string ModifiedBy { get; set; } = string.Empty;

    public DateTime? ModifiedTime { get; set; }
}
