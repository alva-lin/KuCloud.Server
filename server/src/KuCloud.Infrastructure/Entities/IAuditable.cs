namespace KuCloud.Infrastructure.Entities;

public interface IAuditable
{
    public string CreatedBy { get; set; }

    public DateTime CreatedTime { get; set; }

    public string ModifiedBy { get; set; }

    public DateTime? ModifiedTime { get; set; }
}
