using KuCloud.Core.Exceptions;
using KuCloud.Data;
using KuCloud.Data.Models.Storage;
using KuCloud.Infrastructure.Enums;
using KuCloud.Infrastructure.Exceptions;
using KuCloud.Infrastructure.Extensions;
using KuCloud.ObjectStorage.Abstract;
using KuCloud.Services.Abstract.Storage;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
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

    private readonly IMemoryCache _cache;

    public FileService(KuCloudDbContext dbContext, ILogger<FileService> logger, IObjectStorageService objectStorageService, IFolderService folderService, IMemoryCache cache)
    {
        _dbContext            = dbContext;
        _logger               = logger;
        _objectStorageService = objectStorageService;
        _folderService        = folderService;
        _cache                = cache;

        _files = _dbContext.Files.AsQueryable();
    }

    public Task<File?> QueryAsync(string path, string name, CancellationToken cancellationToken = default)
    {
        path = path.TrimEnd(StorageNode.DELIMITER);
        name = name.Trim(StorageNode.DELIMITER);

        return _dbContext.Files.Where(file => file.Path == path && file.Name == name).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<File> FindAsync(string path, string name, CancellationToken cancellationToken = default)
    {
        return await QueryAsync(path, name, cancellationToken) ?? throw new EntityNotFoundException(typeof(File), $"{path}/{name}");
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

    public async Task MoveAsync(string path, string name, string newPath, CancellationToken cancellationToken = default)
    {
        var file = await FindAsync(path, name, cancellationToken);

        Folder? newFolder = null;
        if (newPath.TrimEnd(StorageNode.DELIMITER).IsNotNullOrWhiteSpace())
        {
            newFolder = await _folderService.FindAsync(path, false, cancellationToken);
        }

        file.Parent = newFolder;
        file.SetPath(newFolder);

        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task RenameAsync(string path, string name, string newName, CancellationToken cancellationToken = default)
    {

        if (await QueryAsync(path, newName, cancellationToken) != null)
        {
            throw new BasicException(ErrorCode.ServiceFail, "a file with the same name has already exist");
        }
        var file = await FindAsync(path, name, cancellationToken);
        file.Name = newName;
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<string> CreateUploadUrlAsync(string path, string name, CancellationToken cancellationToken = default)
    {
        var random = new Random();
        var chars  = "QWERTYUIOPASDFGHJKLZXCVBNM".ToCharArray();

        var cosPath = DateTime.UtcNow.ToString("yyyy_MMdd_hhmmss_") + new string(Enumerable.Range(1, 8).Select(_ => chars[random.Next(chars.Length)]).ToArray());

        var url = await _objectStorageService.GenerateSignalUrl(SignalUrlType.Upload, cosPath, cancellationToken);

        _cache.Set(url, new CacheUploadFile(cosPath, path, name), TimeSpan.FromSeconds(2 * 3600));

        return url;
    }

    public async Task UploadDoneAsync(string uploadUrl, CancellationToken cancellationToken = default)
    {
        if (_cache.TryGetValue(uploadUrl, out CacheUploadFile value))
        {
            Folder? folder = null;
            if (value.Path.TrimEnd(StorageNode.DELIMITER).IsNotNullOrWhiteSpace())
            {
                folder = await _folderService.FindAsync(value.Path, false, cancellationToken);
            }

            var file = new File(folder, value.Name)
            {
                StoragePath = value.CosPath,
                UploadTime  = DateTime.UtcNow
            };

            await _dbContext.Files.AddAsync(file, cancellationToken);
        }
    }

    public async Task<string> CreateDownloadUrlAsync(string path, string name, CancellationToken cancellationToken = default)
    {
        var file = await FindAsync(path, name, cancellationToken);

        var cosPath = file.StoragePath;
        var url     = await _objectStorageService.GenerateSignalUrl(SignalUrlType.Download, cosPath, cancellationToken);

        return url;
    }

    private record CacheUploadFile(string CosPath, string Path, string Name);
}
