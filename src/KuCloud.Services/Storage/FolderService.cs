using KuCloud.Core.Exceptions;
using KuCloud.Data;
using KuCloud.Data.Models.Storage;
using KuCloud.Infrastructure.Enums;
using KuCloud.Infrastructure.Exceptions;
using KuCloud.Infrastructure.Extensions;
using KuCloud.ObjectStorage.Abstract;
using KuCloud.Services.Abstract.Storage;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace KuCloud.Services.Storage;

public class FolderService : IFolderService
{
    private readonly KuCloudDbContext _dbContext;

    private readonly ILogger<FolderService> _logger;

    private readonly IQueryable<Folder> _folders;

    private readonly IObjectStorageService _objectStorageService;

    public FolderService(KuCloudDbContext dbContext, ILogger<FolderService> logger, IObjectStorageService objectStorageService)
    {
        _dbContext = dbContext;
        _logger = logger;
        _objectStorageService = objectStorageService;

        _folders = _dbContext.Folders.AsQueryable();
    }

    public async Task CreateAsync(string fullPath, CancellationToken cancellationToken)
    {
        var (path, name) = SplitPath(fullPath);

        Folder? parent = null;
        if (path.IsNotNullOrWhiteSpace())
        {
            var (parentPath, parentName) = SplitPath(path);
            parent = await _folders.FirstOrDefaultAsync(folder => folder.Path == parentPath && folder.Name == parentName, cancellationToken);
        }

        var folder = new Folder(parent, name);

        await _dbContext.Folders.AddAsync(folder, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("{Action} {Id} {Path}", "create folder", folder.Id, folder.FullPath);
    }

    public async Task RemoveAsync(string fullPath, CancellationToken cancellationToken)
    {
        var folder = await QueryAsync(fullPath, false, cancellationToken);
        if (folder != null)
        {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                // delete all file in cloud storage
                var prefix = folder.FullPath + StorageNode.DELIMITER + "%";
                var files = await _dbContext.Files.Where(file => file.Path.StartsWith(prefix)).ToListAsync(cancellationToken);

                var pathArray = files.Select(file => file.StoragePath).ToArray();
                await _objectStorageService.DeleteAsync(pathArray, cancellationToken);

                _dbContext.Folders.Remove(folder);

                await _dbContext.SaveChangesAsync(cancellationToken);

                await transaction.CommitAsync(cancellationToken);
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync(cancellationToken);
                throw new BasicException(ErrorCode.ServiceFail, "remove folder failed", innerException: e);
            }
            _logger.LogInformation("{Action} {Id} {Path}", "remove folder", folder.Id, folder.FullPath);
        }
    }

    public async Task MoveAsync(string fullPath, string newFullPath, CancellationToken cancellationToken)
    {
        var (newPath, newName) = SplitPath(newFullPath);
        newFullPath = newPath + StorageNode.DELIMITER + newName;

        var folder = await QueryAsync(fullPath, false, cancellationToken);
        Folder? newParent = null;
        if (newPath.IsNotNullOrWhiteSpace())
        {
            var (parentPath, parentName) = SplitPath(newPath);
            newParent = await _folders.FirstOrDefaultAsync(folder => folder.Path == parentPath && folder.Name == parentName, cancellationToken);
            if (newParent == null)
            {
                throw new BasicException(ErrorCode.ServiceFail, $"move folder failed: not existed path {newFullPath}");
            }
        }
        if (folder != null)
        {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                var files = await _dbContext.Files.Where(file => file.Path.StartsWith(folder.FullPath)).ToListAsync(cancellationToken);
                files.ForEach(file => file.Path = newFullPath + file.Path.TrimStart(folder.FullPath));

                var folders = await _dbContext.Folders.Where(folder1 => folder1.Path.StartsWith(folder.FullPath)).ToListAsync(cancellationToken);
                folders.ForEach(folder1 => folder1.Path = newFullPath + folder1.Path.TrimStart(folder.FullPath));

                folder.Name = newName;
                folder.Parent = newParent;
                folder.SetPath(newParent);

                await _dbContext.SaveChangesAsync(cancellationToken);

                await transaction.CommitAsync(cancellationToken);
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync(cancellationToken);
                throw new BasicException(ErrorCode.ServiceFail, "move folder failed", innerException: e);
            }
            _logger.LogInformation("{Action} {Id} {Path}", "move folder", folder.Id, folder.FullPath);
        }
    }

    public Task<Folder?> QueryAsync(string fullPath, bool includeNodes, CancellationToken cancellationToken)
    {
        var (path, name) = SplitPath(fullPath);

        var query = _dbContext.Folders.Where(folder => folder.Path == path && folder.Name == name);
        if (includeNodes)
        {
            query = query.Include(folder => folder.Nodes);
        }

        return query.FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<Folder> FindAsync(string fullPath, bool includeNodes = false, CancellationToken cancellationToken = default)
    {
        var (path, name) = SplitPath(fullPath);
        return await QueryAsync(fullPath, includeNodes, cancellationToken) ?? throw new EntityNotFoundException(typeof(Folder), $"{path}/{name}");
    }

    private (string Path, string Name) SplitPath(string fullPath)
    {
        fullPath = fullPath.TrimEnd(StorageNode.DELIMITER);
        var splits = fullPath.Split(StorageNode.DELIMITER);
        var name = splits[^1];
        var path = string.Join(StorageNode.DELIMITER, splits[..^1]);

        return (path, name);
    }
}
