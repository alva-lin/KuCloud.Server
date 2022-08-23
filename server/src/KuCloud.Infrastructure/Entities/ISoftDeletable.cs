namespace KuCloud.Infrastructure.Entities;

public interface ISoftDeletable
{
    public bool IsDelete { get; set; }

    public DateTime? DeletedTime { get; set; }
}