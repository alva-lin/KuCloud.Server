using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KuCloud.Data.Models.Storage;

public class StorageNodeEntityTypeConfiguration : IEntityTypeConfiguration<StorageNode>
{
    public void Configure(EntityTypeBuilder<StorageNode> builder)
    {
        builder.HasDiscriminator(node => node.NodeType)
            .HasValue<Folder>(StorageNodeType.Folder)
            .HasValue<File>(StorageNodeType.File);

        builder.HasOne(node => node.Parent)
            .WithMany(folder => folder.Nodes)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
