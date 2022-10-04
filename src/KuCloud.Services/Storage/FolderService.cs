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
        _dbContext            = dbContext;
        _logger               = logger;
        _objectStorageService = objectStorageService;

        _folders = _dbContext.Folders.AsQueryable();
    }

    public async Task CreateAsync(string path, string name, CancellationToken cancellationToken)
    {
        path = path.TrimEnd(StorageNode.DELIMITER);
        Folder? parent = null;
        if (path.IsNotNullOrWhiteSpace())
        {
            parent = await _folders.FirstOrDefaultAsync(folder => folder.FullPath == path, cancellationToken);
        }

        var folder = new Folder(parent, name);

        await _dbContext.Folders.AddAsync(folder, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("{Action} {Id} {FullPath}", "create folder", folder.Id, folder.FullPath);
    }

    public async Task RemoveAsync(string path, string name, CancellationToken cancellationToken)
    {
        path = path.TrimEnd(StorageNode.DELIMITER);
        name = name.Trim(StorageNode.DELIMITER);

        var folder = await QueryAsync(path, name, false, cancellationToken);
        if (folder != null)
        {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                // delete all file in cloud storage
                var prefix    = folder.FullPath + StorageNode.DELIMITER + "%";
                var files     = await _dbContext.Files.FromSqlRaw($"SELECT * FROM StorageNode WHERE NodeType = 0 AND PATH LIKE {prefix}").ToListAsync(cancellationToken);
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
            _logger.LogInformation("{Action} {Id} {FullPath}", "remove folder", folder.Id, folder.FullPath);
        }
    }

    public async Task MoveAsync(string path, string name, string newPath, CancellationToken cancellationToken)
    {
        path = path.TrimEnd(StorageNode.DELIMITER);
        name = name.Trim(StorageNode.DELIMITER);

        var     folder    = await QueryAsync(path, name, false, cancellationToken);
        Folder? newParent = null;
        if (newPath.IsNotNullOrWhiteSpace())
        {
            newParent = await _folders.FirstOrDefaultAsync(folder => folder.FullPath == path, cancellationToken);
            if (newParent == null)
            {
                throw new BasicException(ErrorCode.ServiceFail, $"move folder failed: not existed path {newPath}");
            }
        }
        if (folder != null)
        {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                var prefix = folder.FullPath + StorageNode.DELIMITER + "%";
                var files  = await _dbContext.Files.FromSqlRaw($"SELECT * FROM StorageNode WHERE NodeType = 0 AND PATH LIKE {prefix}").ToListAsync(cancellationToken);
                files.ForEach(file => file.Path = newPath + file.Path.TrimStart(folder.FullPath));

                var folders = await _dbContext.Folders.FromSqlRaw($"SELECT * FROM StorageNode WHERE NodeType = 1 AND PATH LIKE {prefix}").ToListAsync(cancellationToken);
                folders.ForEach(folder => folder.Path = newPath + folder.Path.TrimStart(folder.FullPath));

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
            _logger.LogInformation("{Action} {Id} {FullPath}", "move folder", folder.Id, folder.FullPath);
        }
    }

    public async Task RenameAsync(string path, string name, string newName, CancellationToken cancellationToken)
    {
        path = path.TrimEnd(StorageNode.DELIMITER);
        name = name.Trim(StorageNode.DELIMITER);

        var folder = await QueryAsync(path, name, false, cancellationToken);
        if (folder != null)
        {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                var oldPath = folder.FullPath;
                
                folder.Name = newName;
                
                var prefix = folder.FullPath + StorageNode.DELIMITER + "%";
                var files  = await _dbContext.Files.FromSqlRaw($"SELECT * FROM StorageNode WHERE NodeType = 0 AND PATH LIKE {prefix}").ToListAsync(cancellationToken);
                files.ForEach(file => file.Path = folder.FullPath + file.Path.TrimStart(oldPath));

                var folders = await _dbContext.Folders.FromSqlRaw($"SELECT * FROM StorageNode WHERE NodeType = 1 AND PATH LIKE {prefix}").ToListAsync(cancellationToken);
                folders.ForEach(folder => folder.Path = folder.FullPath + folder.Path.TrimStart(oldPath));
                
                await _dbContext.SaveChangesAsync(cancellationToken);

                await transaction.CommitAsync(cancellationToken);
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync(cancellationToken);
                throw new BasicException(ErrorCode.ServiceFail, "rename folder failed", innerException: e);
            }
            _logger.LogInformation("{Action} {Id} {FullPath}", "rename folder", folder.Id, folder.FullPath);
        }
    }

    public Task<Folder?> QueryAsync(string path, string name, bool includeNodes, CancellationToken cancellationToken)
    {
        path = path.TrimEnd(StorageNode.DELIMITER);
        name = name.Trim(StorageNode.DELIMITER);

        var query =  _dbContext.Folders.FromSqlRaw($"SELECT * FROM StorageNode WHERE NodeType = 1 AND PATH = {path} AND Name = {name}");
        if (includeNodes)
        {
            query = query.Include(folder => folder.Nodes);
        }

        return query.FirstOrDefaultAsync(cancellationToken);
    }
}
