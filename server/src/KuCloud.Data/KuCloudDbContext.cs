using KuCloud.Data.Models;
using KuCloud.Infrastructure.Entities;

using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

#pragma warning disable CS8618

namespace KuCloud.Data;

public class KuCloudDbContext : DbContext
{
    private const string APP_USER = "Server";

    private readonly IHttpContextAccessor _httpContextAccessor;

    public KuCloudDbContext(DbContextOptions<KuCloudDbContext> options, IHttpContextAccessor httpContextAccessor = null)
        : base(options)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    #region DbSet

    public virtual DbSet<Account> Accounts { get; set; }

    #endregion

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }

    #region BeforeSaveChanges

    /// <summary>
    ///     保存前的预操作
    /// </summary>
    private void BeforeSaveChanges() =>
        AddAuditInfo();

    /// <summary>
    ///     填入审计信息
    /// </summary>
    private void AddAuditInfo()
    {
        var entities = ChangeTracker.Entries<IAuditable>()
            .Where(entry => entry.State == EntityState.Added || entry.State == EntityState.Modified);

        var utcNow = DateTime.UtcNow;
        var user = _httpContextAccessor?.HttpContext?.User?.Identity?.Name ?? APP_USER;

        foreach (var entity in entities)
        {
            if (entity.State == EntityState.Added)
            {
                entity.Entity.CreatedTime = utcNow;
                entity.Entity.CreatedBy = user;
            }
            if (entity.State == EntityState.Modified)
            {
                entity.Entity.ModifiedTime = utcNow;
                entity.Entity.ModifiedBy = user;
            }
        }
    }

    #region SaveChanges

    public override int SaveChanges()
    {
        BeforeSaveChanges();
        return base.SaveChanges();
    }

    public override int SaveChanges(bool acceptAllChangesOnSuccess)
    {
        BeforeSaveChanges();
        return base.SaveChanges(acceptAllChangesOnSuccess);
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new())
    {
        BeforeSaveChanges();
        return await base.SaveChangesAsync(cancellationToken);
    }

    public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = new())
    {
        BeforeSaveChanges();
        return await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken).ConfigureAwait(false);
    }

    #endregion

    #endregion
}
