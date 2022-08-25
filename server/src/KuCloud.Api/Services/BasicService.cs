using KuCloud.Data;
using KuCloud.Infrastructure.Entities;
using KuCloud.Infrastructure.Exceptions;

using Microsoft.EntityFrameworkCore;

namespace KuCloud.Api.Services;

public abstract class BasicService<TEntity, TKey>
    where TEntity : BaseEntity<TKey>
    where TKey : IEquatable<TKey>
{
    protected readonly KuCloudDbContext DbContext;

    protected readonly DbSet<TEntity> DbSet;

    /// <summary>
    /// 是否立刻保存，类范围内的开关
    /// </summary>
    // ReSharper disable once FieldCanBeMadeReadOnly.Global
    protected bool SaveNow;

    protected BasicService(KuCloudDbContext dbContext)
    {
        DbContext = dbContext;
        DbSet = dbContext.Set<TEntity>();
        SaveNow = false;
    }

    /// <summary>
    /// 配置类范围内的默认开关
    /// </summary>
    /// <param name="saveNow">是否默认立刻保存</param>
    public void SetSaveNow(bool saveNow) => SaveNow = saveNow;

    public Task<TEntity?> FindAsync(TKey id, CancellationToken cancellationToken = default)
    {
        return DbSet.FindAsync(id, cancellationToken).AsTask();
    }

    public async Task<TEntity> GetAsync(TKey id, CancellationToken cancellationToken = default)
    {
        return await FindAsync(id, cancellationToken) ?? throw new EntityNotFoundException(typeof(TEntity), id);
    }

    public Task<List<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return DbSet.ToListAsync(cancellationToken);
    }

    public async Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default, bool? saveNow = null)
    {
        var entry = await DbSet.AddAsync(entity, cancellationToken);
        await SaveChangesAsync(cancellationToken, saveNow);
        return entry.Entity;
    }

    public async Task<TEntity> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default, bool? saveNow = null)
    {
        var entry = DbSet.Update(entity);
        await SaveChangesAsync(cancellationToken, saveNow);
        return entry.Entity;
    }

    public async Task UpdateRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default, bool? saveNow = null)
    {
        DbSet.UpdateRange(entities);
        await SaveChangesAsync(cancellationToken, saveNow);
    }

    public async Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default, bool? saveNow = null)
    {
        DbSet.Remove(entity);
        await SaveChangesAsync(cancellationToken, saveNow);
    }

    public async Task DeleteAsync(TKey id, CancellationToken cancellationToken = default, bool? saveNow = null)
    {
        var entity = await FindAsync(id, cancellationToken);
        if (entity is { })
        {
            await DeleteAsync(entity, cancellationToken, saveNow);
        }
    }

    public async Task DeleteAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default, bool? saveNow = null)
    {
        DbSet.RemoveRange(entities);
        await SaveChangesAsync(cancellationToken, saveNow);
    }

    public async Task DeleteAsync(IEnumerable<TKey> ids, CancellationToken cancellationToken = default, bool? saveNow = null)
    {
        var entities = await DbSet.Where(entity => ids.Contains(entity.Id)).ToListAsync(cancellationToken);
        await DeleteAsync(entities, cancellationToken);
    }

    /// <summary>
    /// 根据传入的 saveNow 值来判断立刻保存
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <param name="saveNow">默认值为 true, 如果传一个空值，那么会根据 this.SaveNow 来判断是否立刻保存</param>
    /// <returns>受影响的实体数量</returns>
    protected async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default, bool? saveNow = true)
    {
        if (saveNow ?? SaveNow)
        {
            return await DbContext.SaveChangesAsync(cancellationToken);
        }
        return 0;
    }
}
