using KuCloud.Infrastructure.Entities;
using KuCloud.Services.Abstractions;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace KuCloud.Services;

public abstract class CrudService<TEntity> : ICrudService<TEntity>, IDisposable where TEntity : class, IBasicEntity
{
    private readonly ILogger _logger;

    protected readonly DbContext DbContext;

    protected readonly DbSet<TEntity> DbSet;

    protected readonly bool SaveNow;

    protected CrudService(DbContext dbContext, ILogger logger)
    {
        DbContext = dbContext;
        DbSet     = dbContext.Set<TEntity>();
        SaveNow   = true;
        _logger    = logger;
    }

    DbContext ICrudService<TEntity>.DbContext => DbContext;

    DbSet<TEntity> ICrudService<TEntity>.DbSet => DbSet;

    bool ICrudService<TEntity>.SaveNow => SaveNow;

    ILogger ICrudService<TEntity>.Logger => _logger;

    public ICrudService<TEntity> AsCrudService()
    {
        return this;
    }

    public void Dispose()
    {
        AsCrudService().SaveChangesAsync(true).Wait();
    }
}

public abstract class CrudService<TEntity, TKey> : CrudService<TEntity>, ICrudService<TEntity, TKey>
    where TEntity : BasicEntity<TKey>
    where TKey : IEquatable<TKey>
{
    protected CrudService(DbContext dbContext, ILogger logger)
        : base(dbContext, logger)
    {
    }

    // ReSharper disable once MemberCanBeProtected.Global
    public new ICrudService<TEntity, TKey> AsCrudService()
    {
        return this;
    }
}
