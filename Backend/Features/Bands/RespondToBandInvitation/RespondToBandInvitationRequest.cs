using PrjA.Domain.Entities;

namespace PrjA.Features.Bands.RespondToBandInvitation;

public class RespondToBandInvitationRequest
{
    public InvitationStatus Status { get; set; }
}
