using System.ComponentModel.DataAnnotations.Schema;

namespace KuCloud.Infrastructure.Entities;

// TODO DbContext SoftDelete 修改
public abstract class SoftDeleteEntity : BasicEntity, ISoftDelete
{
    public bool IsDelete { get; set; }

    public string DeletedBy { get; set; } = string.Empty;

    public DateTime DeletedTime { get; set; }
}

public abstract class SoftDeleteEntity<TKey> : SoftDeleteEntity, IBasicEntity<TKey> where TKey : IEquatable<TKey>
{
    public TKey Id { get; set; } = default!;
}
