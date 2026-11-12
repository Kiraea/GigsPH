using PrjA.Domain.Entities;

namespace PrjA.Features.Bands.AcceptInvitation;

public class AcceptInvitationRequest
{
    public Guid InvitationId { get; set; }
    public InvitationStatus Status { get; set; }
}
