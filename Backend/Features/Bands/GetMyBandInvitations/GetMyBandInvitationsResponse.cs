namespace PrjA.Features.Bands.GetMyBandInvitations;

public class GetMyBandInvitationResponse
{
    public Guid InvitationId { get; set; }
    public Guid BandId { get; set; }
    public required string BandName { get; set; }
    public string? BandDescription { get; set; }
    public int MemberCount { get; set; }
    public required string InvitedByUserId { get; set; }
    public required string InvitedByDisplayName { get; set; }
    public DateTime CreatedAt { get; set; }
}
