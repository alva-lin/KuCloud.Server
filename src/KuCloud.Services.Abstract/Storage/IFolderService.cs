using KuCloud.Data.Models.Storage;
using KuCloud.Infrastructure.Common;

namespace KuCloud.Services.Abstract.Storage;

public interface IFolderService : IBasicService
{
    public Task CreateAsync(string path, string name, CancellationToken cancellationToken = default);

    public Task RemoveAsync(string path, string name, CancellationToken cancellationToken = default);

    public Task MoveAsync(string path, string name, string newPath, CancellationToken cancellationToken = default);

    public Task RenameAsync(string path, string name, string newName, CancellationToken cancellationToken = default);

    public Task<Folder?> QueryAsync(string path, string name, bool includeNodes = false, CancellationToken cancellationToken = default);
    
    public Task<Folder?> QueryAsync(string fullPath, bool includeNodes = false, CancellationToken cancellationToken = default);

    public Task<Folder> FindAsync(string path, string name, bool includeNodes = false, CancellationToken cancellationToken = default);
    
    public Task<Folder> FindAsync(string fullPath, bool includeNodes = false, CancellationToken cancellationToken = default);
}
