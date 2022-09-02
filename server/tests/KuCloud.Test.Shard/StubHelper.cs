using KuCloud.Infrastructure.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

using MockQueryable.Moq;

using Moq;
#pragma warning disable CS8619
#pragma warning disable CS8600
#pragma warning disable CS8603

namespace KuCloud.Test.Shard;

public static class StubHelper
{
    public static Mock<DbSet<TEntity>> CreateStubDbSet<TEntity>(List<TEntity> data)
        where TEntity : class, IBasicEntity
    {
        var stubDbSet = data.AsQueryable().BuildMockDbSet();

        stubDbSet.Setup(dbSet => dbSet.AddAsync(It.IsAny<TEntity>(), It.IsAny<CancellationToken>()))
            .Callback((TEntity entity, CancellationToken _) => { data.Add(entity); })
            .Returns((TEntity _, CancellationToken _) => ValueTask.FromResult((EntityEntry<TEntity>)null));
        
        stubDbSet.Setup(dbSet => dbSet.AddAsync(It.IsAny<TEntity>(), It.IsAny<CancellationToken>()))
            .Callback((TEntity entity, CancellationToken _) => { data.Add(entity); })
            .Returns((TEntity _, CancellationToken _) => ValueTask.FromResult((EntityEntry<TEntity>)null));
        
        stubDbSet.Setup(dbSet => dbSet.Update(It.IsAny<TEntity>()))
            .Callback((TEntity entity) =>
            {
                var index = data.FindIndex(entity1 => entity1.Id.Equals(entity.Id));
                if (index == -1)
                {
                    data.Add(entity);
                }
                else
                {
                    data[index] = entity;
                }
            })
            .Returns((TEntity _) => null);

        stubDbSet.Setup(dbSet => dbSet.UpdateRange(It.IsAny<IEnumerable<TEntity>>()))
            .Callback((IEnumerable<TEntity> entities) =>
            {
                foreach (var entity in entities)
                {
                    var index = data.FindIndex(entity1 => entity1.Id.Equals(entity.Id));
                    if (index == -1)
                    {
                        data.Add(entity);
                    }
                    else
                    {
                        data[index] = entity;
                    }
                }
            });

        return stubDbSet;
    }

    public static Mock<DbContext> CreateStubDbContext<TEntity>(Mock<DbSet<TEntity>> dbSet)
        where TEntity : class, IBasicEntity
    {
        var stub = new Mock<DbContext>();

        stub.Setup(context => context.SaveChangesAsync(It.IsAny<CancellationToken>()).Result).Returns(1);

        stub.Setup(context => context.Set<TEntity>()).Returns(dbSet.Object);

        return stub;
    }

    public static Mock<DbContext> CreateStubDbContext<TEntity>(List<TEntity> data)
        where TEntity : class, IBasicEntity
    {
        var stubDbSet = CreateStubDbSet(data);
        var stubDbContext = CreateStubDbContext(stubDbSet);
        return stubDbContext;
    }
}
