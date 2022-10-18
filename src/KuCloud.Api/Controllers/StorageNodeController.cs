using KuCloud.Data.Models.Storage;
using KuCloud.Data.ViewModels.Storage;
using KuCloud.Infrastructure.Common;
using KuCloud.Services.Abstract.Storage;

using Microsoft.AspNetCore.Mvc;

namespace KuCloud.Api.Controllers;

public class StorageNodeController : BasicController
{
    /// <summary>
    /// 获取文件夹详情
    /// </summary>
    /// <param name="path">文件夹路径</param>
    /// <param name="folderService"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet]
    public Task<Folder> Get([FromQuery] string path,
        [FromServices] IFolderService folderService,
        CancellationToken cancellationToken)
    {
        return folderService.FindAsync(path, true, cancellationToken);
    }

    /// <summary>
    /// 创建文件夹
    /// </summary>
    /// <param name="path">文件夹路径</param>
    /// <param name="folderService"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost]
    public Task Create([FromBody] string path,
        [FromServices] IFolderService folderService,
        CancellationToken cancellationToken)
    {
        return folderService.CreateAsync(path, cancellationToken);
    }

    /// <summary>
    /// 移动文件/文件夹
    /// </summary>
    /// <param name="models"></param>
    /// <param name="folderService"></param>
    /// <param name="fileService"></param>
    /// <param name="cancellationToken"></param>
    [HttpPost("[action]")]
    public async Task Move([FromBody] StorageMoveModel[] models,
        [FromServices] IFolderService folderService,
        [FromServices] IFileService fileService,
        CancellationToken cancellationToken)
    {
        var folders = models.Where(model => model.NodeType == StorageNodeType.Folder).ToArray();
        var files = models.Where(model => model.NodeType == StorageNodeType.File).ToArray();

        foreach (var folder in folders)
        {
            await folderService.MoveAsync(folder.Path, folder.NewPath, cancellationToken);
        }
        foreach (var file in files)
        {
            await fileService.MoveAsync(file.Path, file.NewPath, cancellationToken);
        }
    }

    /// <summary>
    /// 删除文件/文件夹
    /// </summary>
    /// <param name="models"></param>
    /// <param name="folderService"></param>
    /// <param name="fileService"></param>
    /// <param name="cancellationToken"></param>
    [HttpDelete]
    public async Task Remove([FromBody] StorageRemoveModel[] models,
        [FromServices] IFolderService folderService,
        [FromServices] IFileService fileService,
        CancellationToken cancellationToken)
    {
        var folders = models.Where(model => model.NodeType == StorageNodeType.Folder).ToArray();
        var files = models.Where(model => model.NodeType == StorageNodeType.File).ToArray();

        foreach (var folder in folders)
        {
            await folderService.RemoveAsync(folder.Path, cancellationToken);
        }
        foreach (var file in files)
        {
            await fileService.RemoveAsync(file.Path, cancellationToken);
        }
    }

    /// <summary>
    /// 生成上传文件的链接
    /// </summary>
    /// <param name="path">文件路径</param>
    /// <param name="fileService"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost("upload")]
    public Task<string> GenerateUploadUrl([FromBody] string path,
        [FromServices] IFileService fileService,
        CancellationToken cancellationToken)
    {
        return fileService.CreateUploadUrlAsync(path, cancellationToken);
    }

    /// <summary>
    /// 文件上传完成
    /// </summary>
    /// <param name="url">文件上传链接</param>
    /// <param name="fileService"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpPost("upload/done")]
    public Task UploadDone([FromBody] string url,
        [FromServices] IFileService fileService,
        CancellationToken cancellationToken)
    {
        return fileService.UploadDoneAsync(url, cancellationToken);
    }

    /// <summary>
    /// 生成下载文件的链接
    /// </summary>
    /// <param name="path">文件路径</param>
    /// <param name="fileService"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    [HttpGet("download")]
    public Task<string> GenerateDownloadUrl([FromQuery] string path,
        [FromServices] IFileService fileService,
        CancellationToken cancellationToken)
    {
        return fileService.CreateDownloadUrlAsync(path, cancellationToken);
    }
}
