using KuCloud.Infrastructure.Entities;
using KuCloud.Infrastructure.Extensions;

using Microsoft.EntityFrameworkCore;

using System.ComponentModel.DataAnnotations;

namespace KuCloud.Data.Models.Storage;

/// <summary>
/// 存储节点
/// </summary>
[Index(nameof(NodeType), nameof(Path), nameof(Name), IsUnique = true)]
public abstract class StorageNode : BasicEntity<long>
{
    public const string DELIMITER = "/";

    /// <summary>
    /// 路径
    /// </summary>
    public string Path { get; set; } = null!;

    /// <summary>
    /// 名称
    /// </summary>
    public string Name { get; set; } = null!;

    public string FullPath => Path + DELIMITER + Name;

    /// <summary>
    /// 节点类别
    /// </summary>
    public StorageNodeType NodeType { get; protected init; }

    /// <summary>
    /// 父节点
    /// </summary>
    public Folder? Parent { get; set; }
    
    protected StorageNode() {}

    protected StorageNode(Folder? parent, string name)
    {
        name = name.Trim(DELIMITER);

        Parent   = parent;
        Name     = name.Trim(DELIMITER);

        SetPath(parent);
    }

    public void SetPath(Folder? parent)
    {
        Path = parent?.FullPath ?? string.Empty;
    }
}
