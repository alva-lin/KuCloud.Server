using KuCloud.Data.Models.Storage;

namespace KuCloud.Data.ViewModels.Storage;

public class StorageMoveModel
{
    /// <summary>
    /// 原路径
    /// </summary>
    public string Path { get; set; } = null!;

    /// <summary>
    /// 新路径
    /// </summary>
    public string NewPath { get; set; } = null!;

    /// <summary>
    /// 类型
    /// </summary>
    public StorageNodeType NodeType { get; set; }
}

public class StorageRemoveModel
{
    /// <summary>
    /// 路径
    /// </summary>
    public string Path { get; set; } = null!;

    /// <summary>
    /// 类型
    /// </summary>
    public StorageNodeType NodeType { get; set; }
}
