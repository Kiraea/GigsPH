namespace PrjA.Domain.Entities;

public class MediaUpload
{
    public Guid Id { get; set; } =  Guid.CreateVersion7();
    
    public required string RequestedByUserId { get; set; }
    public required string ObjectKey { get; set; }
    
    public required string FileName { get; set; }
    public required string ContentType { get; set; }
    public required long Size { get; set; }
    public UploadPurpose Purpose { get; set; }
    public UploadStatus Status { get; set; }
}

public enum UploadStatus
{
    Pending,
    Attached,
}

public enum UploadPurpose
{
    Post,
    Profile,
}