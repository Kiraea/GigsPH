using PrjA.Domain.Entities;

namespace PrjA.Features.Uploads;

public class UploadRequest
{
    public required string FileName { get; set; }
    public long FileSize { get; set; }
    public required string FileType { get; set; }
    public required UploadPurpose Purpose { get; set; }
}