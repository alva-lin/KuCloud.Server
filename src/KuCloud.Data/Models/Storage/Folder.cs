namespace KuCloud.Data.Models.Storage;

/// <summary>
/// 文件夹
/// </summary>
public class Folder : StorageNode
{
    /// <summary>
    /// 子节点
    /// </summary>
    public IReadOnlyList<StorageNode> Nodes { get; protected set; } = new List<StorageNode>();
    
    protected Folder() {}

    public Folder(Folder? parent, string name)
        : base(parent, name)
    {
        NodeType = StorageNodeType.Folder;
    }
}
