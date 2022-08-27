using KuCloud.Core.Exceptions;
using KuCloud.Infrastructure.Entities;
using KuCloud.Services.Abstractions;

using Microsoft.EntityFrameworkCore;

using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

namespace KuCloud.Services;

public abstract class BasicEntityService<TEntity> : IBasicEntityService<TEntity> where TEntity : class, IBasicEntity
{
    protected BasicEntityService(DbContext dbContext)
    {
        DbContext = dbContext;
        DbSet = dbContext.Set<TEntity>();
        SaveNow = true;
    }

    public DbContext DbContext { get; set; }

    public DbSet<TEntity> DbSet { get; set; }

    public bool SaveNow { get; set; }

    /// <summary>
    ///     根据传入的 saveNow 值来判断立刻保存
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <param name="saveNow">默认值为 true, 如果传一个空值，那么会根据 this.SaveNow 来判断是否立刻保存</param>
    /// <returns>受影响的实体数量</returns>
    public virtual async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default, bool? saveNow = true)
    {
        if (saveNow ?? SaveNow)
        {
            return await DbContext.SaveChangesAsync(cancellationToken);
        }
        return 0;
    }

    public virtual Task<List<TEntity>> GetAllAsync(CancellationToken cancellationToken = default) =>
        DbSet.ToListAsync(cancellationToken);

    public Task<List<TEntity>> WhereAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken)
    {
        return DbSet.Where(predicate).ToListAsync(cancellationToken: cancellationToken);
    }

    public Task<TEntity?> FindAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken)
    {
        return DbSet.FirstOrDefaultAsync(predicate, cancellationToken);
    }

    public async Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken)
    {
        return await FindAsync(predicate, cancellationToken) ?? throw new EntityNotFoundException(typeof(TEntity), predicate);
    }

    public virtual async Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default, bool? saveNow = null)
    {
        var entry = await DbSet.AddAsync(entity, cancellationToken);
        await SaveChangesAsync(cancellationToken, saveNow);
        return entry.Entity;
    }

    public virtual async Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default, bool? saveNow = null)
    {
        var entry = DbSet.Update(entity);
        await SaveChangesAsync(cancellationToken, saveNow);
        return entry.Entity;
    }

    public virtual async Task UpdateRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default, bool? saveNow = null)
    {
        DbSet.UpdateRange(entities);
        await SaveChangesAsync(cancellationToken, saveNow);
    }

    public virtual async Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default, bool? saveNow = null)
    {
        DbSet.Remove(entity);
        await SaveChangesAsync(cancellationToken, saveNow);
    }

    public virtual async Task DeleteAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default, bool? saveNow = null)
    {
        DbSet.RemoveRange(entities);
        await SaveChangesAsync(cancellationToken, saveNow);
    }
}

public abstract class BasicEntityService<TEntity, TKey> : BasicEntityService<TEntity>, IBasicEntityService<TEntity, TKey>
    where TEntity : BasicEntity<TKey>
    where TKey : IEquatable<TKey>
{
    protected BasicEntityService(DbContext dbContext)
        : base(dbContext)
    {
    }

    public virtual Task<TEntity?> FindAsync(TKey id, CancellationToken cancellationToken = default) =>
        DbSet.FindAsync(id, cancellationToken).AsTask();

    public virtual async Task<TEntity> GetAsync(TKey id, CancellationToken cancellationToken = default) =>
        await FindAsync(id, cancellationToken) ?? throw new EntityNotFoundException(typeof(TEntity), id);

    public virtual async Task DeleteAsync(TKey id, CancellationToken cancellationToken = default, bool? saveNow = null)
    {
        var entity = await FindAsync(id, cancellationToken);
        if (entity is not null)
        {
            await DeleteAsync(entity, cancellationToken, saveNow);
        }
    }

    public virtual async Task DeleteAsync(IEnumerable<TKey> ids, CancellationToken cancellationToken = default, bool? saveNow = null)
    {
        var entities = await DbSet.Where(entity => ids.Contains(entity.Id)).ToListAsync(cancellationToken);
        await DeleteAsync(entities, cancellationToken);
    }
}
