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

    private readonly IObjectStorageService _objectStorageService;

    private readonly IFolderService _folderService;

    private readonly IMemoryCache _cache;

    public FileService(KuCloudDbContext dbContext, IObjectStorageService objectStorageService, IFolderService folderService, IMemoryCache cache, ILogger<FileService> logger)
    {
        _dbContext = dbContext;
        _objectStorageService = objectStorageService;
        _folderService = folderService;
        _cache = cache;
        _logger = logger;
    }

    private (string Path, string Name) SplitPath(string fullPath)
    {
        fullPath = fullPath.TrimEnd(StorageNode.DELIMITER);
        var splits = fullPath.Split(StorageNode.DELIMITER);
        var name = splits[^1];
        var path = string.Join(StorageNode.DELIMITER, splits[..^1]);

        return (path, name);
    }

    public Task<File?> QueryAsync(string path, CancellationToken cancellationToken = default)
    {
        var (folderPath, name) = SplitPath(path);

        return _dbContext.Files.Where(file => file.Path == folderPath && file.Name == name).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<File> FindAsync(string path, CancellationToken cancellationToken = default)
    {
        return await QueryAsync(path, cancellationToken) ?? throw new EntityNotFoundException(typeof(File), path);
    }

    public async Task RemoveAsync(string path, CancellationToken cancellationToken = default)
    {
        var file = await QueryAsync(path, cancellationToken);
        if (file != null)
        {
            _dbContext.Files.Remove(file);
            await _dbContext.SaveChangesAsync(cancellationToken);

            await _objectStorageService.DeleteAsync(file.StoragePath, cancellationToken);

            _logger.LogInformation("{Action} {Id} {Path}", "remove file", file.Id, file.FullPath);
        }
    }

    public async Task MoveAsync(string path, string newPath, CancellationToken cancellationToken = default)
    {
        var file = await FindAsync(path, cancellationToken);
        if (await QueryAsync(newPath, cancellationToken) != null)
        {
            throw new BasicException(ErrorCode.ServiceFail, "a file with the same name has already exist");
        }

        var (folderPath, name) = SplitPath(newPath);
        Folder? newFolder = null;
        if (newPath.TrimEnd(StorageNode.DELIMITER).IsNotNullOrWhiteSpace())
        {
            newFolder = await _folderService.FindAsync(folderPath, false, cancellationToken);
        }

        file.Parent = newFolder;
        file.Name = name;
        file.SetPath(newFolder);

        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("{Action} {Id} {Path}", "move file", file.Id, file.FullPath);
    }

    public async Task<string> CreateUploadUrlAsync(string path, CancellationToken cancellationToken = default)
    {
        var random = new Random();
        var chars = "QWERTYUIOPASDFGHJKLZXCVBNM".ToCharArray();

        var cosPath = DateTime.UtcNow.ToString("yyyy_MMdd_HHmmss_") + new string(Enumerable.Range(1, 8).Select(_ => chars[random.Next(chars.Length)]).ToArray());

        var url = await _objectStorageService.GenerateSignalUrl(SignalUrlType.Upload, cosPath, cancellationToken: cancellationToken);

        _cache.Set(url, new CacheUploadFile(cosPath, path), TimeSpan.FromSeconds(2 * 3600));

        _logger.LogInformation("{Action} {UrlType} {Path}", "generate signal url", SignalUrlType.Upload, path);

        return url;
    }

    public async Task UploadDoneAsync(string uploadUrl, CancellationToken cancellationToken = default)
    {
        if (_cache.TryGetValue(uploadUrl, out CacheUploadFile value))
        {
            var (folderPath, name) = SplitPath(value.Path);

            Folder? folder = null;
            if (folderPath.TrimEnd(StorageNode.DELIMITER).IsNotNullOrWhiteSpace())
            {
                folder = await _folderService.FindAsync(folderPath, false, cancellationToken);
            }
            

            var file = new File(folder, name)
            {
                StoragePath = value.CosPath,
                UploadTime = DateTime.UtcNow
            };
            
            var info = await _objectStorageService.GetInfoAsync(value.CosPath, cancellationToken);
            info.TryGetValue("Content-Length", out var temp);
            int.TryParse(temp?.FirstOrDefault() ?? "0", out var fileSize);
            file.Size = fileSize;

            await _dbContext.Files.AddAsync(file, cancellationToken);
            await _dbContext.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("{Action} {Id} {Path}", "upload file done", file.Id, file.FullPath);
        }
    }

    public async Task<string> CreateDownloadUrlAsync(string path, CancellationToken cancellationToken = default)
    {
        var file = await FindAsync(path, cancellationToken);

        var cosPath = file.StoragePath;
        var url = await _objectStorageService.GenerateSignalUrl(SignalUrlType.Download, cosPath, file.Name, cancellationToken);

        _logger.LogInformation("{Action} {UrlType} {Path}", "generate signal url", SignalUrlType.Download, path);

        return url;
    }

    private record CacheUploadFile(string CosPath, string Path);
}
