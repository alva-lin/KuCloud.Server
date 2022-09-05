namespace KuCloud.Infrastructure.Entities;

public interface IBasicEntity
{
    public object Id { get; set; }
}

public interface IBasicEntity<TKey> : IBasicEntity, IAuditable where TKey : IEquatable<TKey>
{
    public new TKey Id { get; set; }

    object IBasicEntity.Id
    {
        get => Id;
        set { }
    }
}
