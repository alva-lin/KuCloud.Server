namespace KuCloud.Data.Models.Storage;

public class File : StorageNode
{
    /// <summary>
    /// 云存储中的存储路径
    /// </summary>
    public string StoragePath { get; protected set; } = null!;

    /// <summary>
    /// 文件上传时间
    /// </summary>
    public DateTime UploadTime { get; protected set; }

    /// <summary>
    /// 文件大小
    /// </summary>
    public long Size { get; protected set; }

    public File(Folder? parent, string name)
        : base(parent, name)
    {
        NodeType = StorageNodeType.File;
    }
}