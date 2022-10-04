using KuCloud.Core.Exceptions;
using KuCloud.Infrastructure.Common;
using KuCloud.Infrastructure.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Logging;

using System.Linq.Expressions;

namespace KuCloud.Services.Abstract;

public interface ICrudService<TEntity> : IBasicService where TEntity : class, IBasicEntity
{
    protected DbContext DbContext { get; }

    protected DbSet<TEntity> DbSet { get; }

    protected bool SaveNow { get; }

    protected ILogger Logger { get; }

    /// <summary>
    ///     根据传入的 saveNow 值来判断立刻保存
    ///     <br/>
    ///     在释放时，会自动执行一次 SaveChangesAsync(saveNow: true)
    /// </summary>
    /// <param name="saveNow">默认值为 true, 如果传一个空值，那么会根据 this.SaveNow 来判断是否立刻保存</param>
    /// <param name="cancellationToken"></param>
    /// <returns>受影响的实体数量</returns>
    public async Task<int> SaveChangesAsync(bool? saveNow = null, CancellationToken cancellationToken = default)
    {
        if (saveNow ?? SaveNow)
        {
            return await DbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }
        return 0;
    }

    public Task<List<TEntity>> WhereAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return DbSet.Where(predicate).ToListAsync(cancellationToken: cancellationToken);
    }

    public Task<TEntity?> FindAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return DbSet.FirstOrDefaultAsync(predicate, cancellationToken);
    }

    public async Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken)
    {
        return await FindAsync(predicate, cancellationToken) ?? throw new EntityNotFoundException(typeof(TEntity), predicate);
    }

    public async Task<EntityEntry<TEntity>> AddAsync(TEntity entity, bool? saveNow = null, CancellationToken cancellationToken = default)
    {
        var entry = await DbSet.AddAsync(entity, cancellationToken);
        await SaveChangesAsync(saveNow, cancellationToken);
        Logger.LogInformation("add type={@Type} saveNow={@SaveNow} data={@Data}", entity.GetType(), saveNow, entity);
        return entry;
    }

    public async Task<EntityEntry<TEntity>> UpdateAsync(TEntity entity, bool? saveNow = null, CancellationToken cancellationToken = default)
    {
        var entry = DbSet.Update(entity);
        await SaveChangesAsync(saveNow, cancellationToken);
        Logger.LogInformation("update type={@Type} saveNow={@SaveNow} data={@Data}", entity.GetType(), saveNow, entity);
        return entry;
    }

    public async Task UpdateAsync(IEnumerable<TEntity> entities, bool? saveNow = null, CancellationToken cancellationToken = default)
    {
        DbSet.UpdateRange(entities);
        await SaveChangesAsync(saveNow, cancellationToken);
        Logger.LogInformation("update type={@Type} saveNow={@SaveNow} data={@Data}", typeof(TEntity), saveNow, entities);
    }

    public async Task DeleteAsync(TEntity entity, bool? saveNow = null, CancellationToken cancellationToken = default)
    {
        DbSet.Remove(entity);
        await SaveChangesAsync(saveNow, cancellationToken);
        Logger.LogInformation("delete type={@Type} saveNow={@SaveNow} data={@Data}", typeof(TEntity), saveNow, entity);
    }

    public async Task DeleteAsync(IEnumerable<TEntity> entities, bool? saveNow = null, CancellationToken cancellationToken = default)
    {
        DbSet.RemoveRange(entities);
        await SaveChangesAsync(saveNow, cancellationToken);
        Logger.LogInformation("delete type={@Type} saveNow={@SaveNow} data={@Data}", typeof(TEntity), saveNow, entities);
    }
}

public interface ICrudService<TEntity, TKey> : ICrudService<TEntity>
    where TEntity : class, IBasicEntity<TKey>
    where TKey : IEquatable<TKey>
{
    public Task<TEntity?> FindAsync(TKey id, CancellationToken cancellationToken = default)
    {
        return DbSet.FindAsync(id, cancellationToken).AsTask();
    }

    public async Task<TEntity> GetAsync(TKey id, CancellationToken cancellationToken = default)
    {
        return await FindAsync(id, cancellationToken) ?? throw new EntityNotFoundException(typeof(TEntity), id);
    }

    public async Task DeleteAsync(TKey id, bool? saveNow = null, CancellationToken cancellationToken = default)
    {
        var entity = await FindAsync(id, cancellationToken);
        if (entity is not null)
        {
            await DeleteAsync(entity, saveNow, cancellationToken);
            Logger.LogInformation("delete type={@Type} saveNow={@SaveNow} data={@Data}", typeof(TEntity), saveNow, new { Id = id });
        }
    }

    public async Task DeleteAsync(IEnumerable<TKey> ids, bool? saveNow = null, CancellationToken cancellationToken = default)
    {
        var entities = await DbSet.Where(entity => ids.Contains(entity.Id)).ToListAsync(cancellationToken);
        await DeleteAsync(entities, saveNow, cancellationToken);
        Logger.LogInformation("delete type={@Type} saveNow={@SaveNow} data={@Data}", typeof(TEntity), saveNow, entities.Select(id => new { Id = id }));
    }
}
