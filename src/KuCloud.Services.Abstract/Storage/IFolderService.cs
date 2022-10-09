using KuCloud.Data.Models.Storage;
using KuCloud.Infrastructure.Common;

namespace KuCloud.Services.Abstract.Storage;

public interface IFolderService : IBasicService
{
    public Task CreateAsync(string fullPath, CancellationToken cancellationToken = default);

    public Task RemoveAsync(string fullPath, CancellationToken cancellationToken = default);

    public Task MoveAsync(string fullPath, string newFullPath, CancellationToken cancellationToken = default);
    
    public Task<Folder?> QueryAsync(string fullPath, bool includeNodes = false, CancellationToken cancellationToken = default);
    
    public Task<Folder> FindAsync(string fullPath, bool includeNodes = false, CancellationToken cancellationToken = default);
}
