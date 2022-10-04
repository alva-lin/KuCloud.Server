using KuCloud.Data;
using KuCloud.Data.Models.Storage;
using KuCloud.Infrastructure.Extensions;
using KuCloud.ObjectStorage.Abstract;
using KuCloud.Services.Abstract.Storage;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using File = KuCloud.Data.Models.Storage.File;

namespace KuCloud.Services.Storage;

public class FileService : IFileService
{
    private readonly KuCloudDbContext _dbContext;

    private readonly ILogger<FileService> _logger;

    private readonly IQueryable<File> _files;

    private readonly IObjectStorageService _objectStorageService;

    private readonly IFolderService _folderService;

    public FileService(KuCloudDbContext dbContext, ILogger<FileService> logger, IObjectStorageService objectStorageService, IFolderService folderService)
    {
        _dbContext            = dbContext;
        _logger               = logger;
        _objectStorageService = objectStorageService;
        _folderService   = folderService;

        _files = _dbContext.Files.AsQueryable();
    }

    public Task<File?> QueryAsync(string path, string name, CancellationToken cancellationToken = default)
    {
        path = path.TrimEnd(StorageNode.DELIMITER);
        name = name.Trim(StorageNode.DELIMITER);

        return _dbContext.Files.FromSqlRaw($"SELECT * FROM StorageNode WHERE NodeType = 0 AND PATH = {path} AND Name = {name}").FirstOrDefaultAsync(cancellationToken);
    }

    public async Task RemoveAsync(string path, string name, CancellationToken cancellationToken = default)
    {
        var file = await QueryAsync(path, name, cancellationToken);
        if (file != null)
        {
            _dbContext.Files.Remove(file);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }

    public Task MoveAsync(string path, string name, string newPath, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Task RenameAsync(string path, string name, string newName, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<string> CreateUploadUrlAsync(string path, string name, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task UploadDoneAsync(string uploadUrl, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<string> CreateDownloadUrlAsync(string path, string name, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}
