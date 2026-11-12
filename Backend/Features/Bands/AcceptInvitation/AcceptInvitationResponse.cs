using PrjA.Domain.Entities;

namespace PrjA.Features.Bands.AcceptInvitation;

public class AcceptInvitationResponse
{
    public Guid InvitationId { get; set; }
    public InvitationStatus Status { get; set; }
    
    public Guid BandId { get; set; }
}
