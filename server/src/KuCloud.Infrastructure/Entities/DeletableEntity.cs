namespace KuCloud.Infrastructure.Entities;

public class DeletableEntity<TKey> : BaseEntity<TKey>, ISoftDeletable where TKey : IEquatable<TKey>
{
    public bool IsDelete { get; set; }

    public DateTime? DeletedTime { get; set; }
}
