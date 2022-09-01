using KuCloud.Infrastructure.Common;
using KuCloud.Infrastructure.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

using System.Linq.Expressions;

namespace KuCloud.Services.Abstractions;

public interface IBasicEntityService<TEntity> : IBasicService where TEntity : class, IBasicEntity
{
    public DbContext DbContext { get; }

    public DbSet<TEntity> DbSet { get; }

    public bool SaveNow { get; set; }

    /// <summary>
    ///     根据传入的 saveNow 值来判断立刻保存
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <param name="saveNow">默认值为 true, 如果传一个空值，那么会根据 this.SaveNow 来判断是否立刻保存</param>
    /// <returns>受影响的实体数量</returns>
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken, bool? saveNow = true);

    public Task<List<TEntity>> GetAllAsync(CancellationToken cancellationToken);

    public Task<List<TEntity>> WhereAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken);

    public Task<TEntity?> FindAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken);

    public Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken);

    public Task<EntityEntry<TEntity>> AddAsync(TEntity entity, CancellationToken cancellationToken, bool? saveNow);

    public Task<EntityEntry<TEntity>> UpdateAsync(TEntity entity, CancellationToken cancellationToken, bool? saveNow);

    public Task UpdateRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken, bool? saveNow);

    public Task DeleteAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken, bool? saveNow);

    public Task DeleteAsync(TEntity entity, CancellationToken cancellationToken, bool? saveNow);
}

public interface IBasicEntityService<TEntity, TKey> : IBasicEntityService<TEntity>
    where TEntity : class, IBasicEntity<TKey>
    where TKey : IEquatable<TKey>
{
    public Task<TEntity?> FindAsync(TKey id, CancellationToken cancellationToken);

    public Task<TEntity> GetAsync(TKey id, CancellationToken cancellationToken);

    public Task DeleteAsync(TKey id, CancellationToken cancellationToken, bool? saveNow);

    public Task DeleteAsync(IEnumerable<TKey> ids, CancellationToken cancellationToken, bool? saveNow);
}
