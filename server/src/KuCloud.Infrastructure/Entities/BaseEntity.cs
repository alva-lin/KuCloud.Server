namespace KuCloud.Infrastructure.Entities;

public class BaseEntity<TKey> : IEntity, IAuditable where TKey : IEquatable<TKey>
{
    public TKey Id { get; set; } = default!;

    object IEntity.Id
    {
        get => Id;
        set { }
    }

    public string CreatedBy { get; set; } = string.Empty;

    public DateTime CreatedTime { get; set; }

    public string ModifiedBy { get; set; } = string.Empty;

    public DateTime? ModifiedTime { get; set; }
}
