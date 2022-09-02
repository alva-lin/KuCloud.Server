using KuCloud.Core.Exceptions;
using KuCloud.Test.Shard;

using System.Linq.Expressions;

namespace KuCloud.Services.Test;

public class BasicEntityServiceTest
{
    private List<MockEntity> _originData;
    private readonly MockEntityService _stubService;

    public BasicEntityServiceTest()
    {
        var data = new List<MockEntity>()
        {
            new() { Id = 1, StringProperty = "Mock1" },
            new() { Id = 2, StringProperty = "Mock2" },
            new() { Id = 3, StringProperty = "Mock3" },
        };
        var mockDbSet = StubHelper.CreateStubDbSet(data);
        var mockDbContext = StubHelper.CreateStubDbContext(mockDbSet);
        _stubService = new(mockDbContext.Object);
        _originData = data;
    }

    // [Fact]
    // public async Task SaveChangesAsync_SaveNow_Null_Return_1()
    // {
    //     // Act
    //     var actual = await _stubService.SaveChangesAsync(saveNow: null);
    //
    //     // Assert
    //     Assert.Equal(1, actual);
    //
    //     _mockDbContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()));
    // }
    //
    // [Fact]
    // public async Task SaveChangesAsync_SaveNow_True_Return_1()
    // {
    //     // Act
    //     var actual = await _stubService.SaveChangesAsync(saveNow: true);
    //
    //     // Assert
    //     Assert.Equal(1, actual);
    //
    //     _mockDbContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()));
    // }
    //
    // [Fact]
    // public async Task SaveChangesAsync_SaveNow_False_Return_0()
    // {
    //     // Act
    //     var actual = await _stubService.SaveChangesAsync(saveNow: false);
    //
    //     // Assert
    //     Assert.Equal(0, actual);
    //
    //     _mockDbContext.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    // }

    [Fact]
    public async Task GetAllAsync_Test()
    {
        // Act
        var actual = await _stubService.GetAllAsync();

        // Assert
        Assert.Equal(_originData, actual);
    }

    [Fact]
    public async Task WhereAsync_Test()
    {
        // Arrange
        Expression<Func<MockEntity, bool>> predicate = entity => entity.Id % 2 == 0;
        var expected = _originData.Where(predicate.Compile()).ToList();

        // Act
        var actual = await _stubService.WhereAsync(predicate);

        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task FindAsync_Test()
    {
        // Arrange
        Expression<Func<MockEntity, bool>> predicate = entity => entity.Id % 2 == 0;
        var expected = _originData.Find(entity => entity.Id % 2 == 0);

        // Act
        var actual = await _stubService.FindAsync(predicate);

        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task GetAsync_FindOne()
    {
        // Arrange
        Expression<Func<MockEntity, bool>> predicate = entity => entity.Id % 2 == 0;
        var expected = _originData.Find(entity => entity.Id % 2 == 0)!;

        // Act
        var actual = await _stubService.GetAsync(predicate);

        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task GetAsync_ThrowEntityNotFoundException()
    {
        // Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(async () =>
        {
            await _stubService.GetAsync(entity => false);
        });
    }

    [Fact]
    public async Task AddAsync_Test()
    {
        // Arrange
        var entity = new MockEntity()
        {
            Id = 9, StringProperty = "Mock Entity"
        };
        var count = _stubService.DbSet.Count();

        // Act
        var _ = await _stubService.AddAsync(entity);
        var newCount = _stubService.DbSet.Count();

        // Assert
        Assert.Equal(count + 1, newCount);
    }

    [Fact]
    public async Task UpdateOrAddAsync_UpdateExistEntity()
    {
        var entity = _originData.First();
        entity.StringProperty = "Modified Property";
        var count = _stubService.DbSet.Count();

        // Act
        var _ = await _stubService.UpdateOrAddAsync(entity);
        var newCount = _stubService.DbSet.Count();

        // Assert
        Assert.Equal(count, newCount);
    }

    [Fact]
    public async Task UpdateOrAddAsync_AddEntity_Count_Plus_One()
    {
        var entity = new MockEntity()
        {
            Id = _originData.Count + 1, StringProperty = "New Property"
        };
        var count = _stubService.DbSet.Count();

        // Act
        var _ = await _stubService.UpdateOrAddAsync(entity);
        var newCount = _stubService.DbSet.Count();

        // Assert
        Assert.Equal(count + 1, newCount);
    }

    [Fact]
    public async Task UpdateOrAddAsync_AddEntities()
    {
        // Arrange
        var addList = new List<MockEntity>()
        {
            new() { Id = _originData.Count + 1, StringProperty = "New Property" },
            new() { Id = _originData.Count + 2, StringProperty = "New Property 2" }
        };
        addList.AddRange(_originData);
        var count = _stubService.DbSet.Count();

        // Act
        await _stubService.UpdateOrAddAsync(addList);
        var newCount = _stubService.DbSet.Count();

        // Assert
        Assert.Equal(count + 2, newCount);
    }

    public async Task DeleteAsync_Test()
    {
        // Arrange
        
    }
}
