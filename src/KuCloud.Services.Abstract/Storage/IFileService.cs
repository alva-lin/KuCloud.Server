using KuCloud.Infrastructure.Common;

using File = KuCloud.Data.Models.Storage.File;

namespace KuCloud.Services.Abstract.Storage;

public interface IFileService : IBasicService
{
    public Task<File?> QueryAsync(string path, string name, CancellationToken cancellationToken = default);

    public Task<File> FindAsync(string path, string name, CancellationToken cancellationToken = default);

    public Task RemoveAsync(string path, string name, CancellationToken cancellationToken = default);

    public Task MoveAsync(string path, string name, string newPath, CancellationToken cancellationToken = default);

    public Task RenameAsync(string path, string name, string newName, CancellationToken cancellationToken = default);

    public Task<string> CreateUploadUrlAsync(string path, string name, CancellationToken cancellationToken = default);

    public Task UploadDoneAsync(string uploadUrl, CancellationToken cancellationToken = default);

    public Task<string> CreateDownloadUrlAsync(string path, string name, CancellationToken cancellationToken = default);
}
