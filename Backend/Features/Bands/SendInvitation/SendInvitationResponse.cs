using PrjA.Domain.Entities;

namespace PrjA.Features.Bands.SendInvitation;

public class SendInvitationResponse
{
    
    public Guid InvitationId { get; set; }
    public Guid BandId { get; set; }
    public required string ToId { get; set; }
    public InvitationStatus Status { get; set; }
    
}