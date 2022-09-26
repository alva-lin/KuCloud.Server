using System.ComponentModel.DataAnnotations.Schema;
#pragma warning disable CS8618

namespace KuCloud.Infrastructure.Entities;

public abstract class BasicEntity : IBasicEntity
{
    public string CreatedBy { get; set; } = string.Empty;

    public DateTime CreatedTime { get; set; }

    public string ModifiedBy { get; set; } = string.Empty;

    public DateTime? ModifiedTime { get; set; }
}

public abstract class BasicEntity<TKey> : BasicEntity, IBasicEntity<TKey> where TKey : IEquatable<TKey>
{
    public TKey Id { get; set; }
}
