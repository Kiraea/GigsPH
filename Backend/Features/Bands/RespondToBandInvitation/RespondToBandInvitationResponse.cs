using PrjA.Domain.Entities;

namespace PrjA.Features.Bands.RespondToBandInvitation;

public class RespondToBandInvitationResponse
{
    public Guid InvitationId { get; set; }
    public InvitationStatus Status { get; set; }
    
    public Guid BandId { get; set; }
}
