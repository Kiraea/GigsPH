namespace PrjA.Features.Uploads;

public class UploadResponse
{
    public Guid UploadId { get; set; }
    public required string UploadUrl { get; set; }
}