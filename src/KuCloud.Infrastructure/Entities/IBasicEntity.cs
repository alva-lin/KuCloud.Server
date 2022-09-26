namespace KuCloud.Infrastructure.Entities;

public interface IBasicEntity 
{
}

public interface IBasicEntity<TKey> : IBasicEntity, IAuditable where TKey : IEquatable<TKey>
{
    public TKey Id { get; set; }
}
