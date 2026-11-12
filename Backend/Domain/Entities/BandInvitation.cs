namespace PrjA.Domain.Entities;

public class BandInvitation
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public Guid BandId { get; set; }
    public required string InvitedByUserId { get; set; }
    public required string InvitedToUserId { get; set; }
    public InvitationStatus Status { get; set; } 
}



public enum InvitationStatus
{
    Accepted,
    Rejected,
    Pending,
    Cancelled,
}