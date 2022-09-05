using KuCloud.Services;

using Microsoft.EntityFrameworkCore;

namespace KuCloud.Test.Shard;

public class MockEntityService : BasicEntityService<MockEntity, int>
{
    public MockEntityService(DbContext dbContext) : base(dbContext) {}
}
