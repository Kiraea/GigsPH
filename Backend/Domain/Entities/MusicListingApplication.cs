namespace PrjA.Domain.Entities;

public class MusicListingApplication : ITimestamp
{
    public Guid Id { get; set; }
    public MusicListingSlot?  MusicListingSlot { get; set; }
    public Guid MusicListingSlotId { get; set; }
    public required string ApplicantUserId { get; set; }
    public AppUser? ApplicantUser { get; set; }
    public MusicListingApplicationStatus Status { get; set; }
    public string? Note { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime CreatedAt { get; set; }
}


public enum MusicListingApplicationStatus
{
    Pending,
    Accepted,
    Withdrawn
}
