using PrjA.Domain;

namespace PrjA.Features.Bands.UpdateBandMemberRole;

public class UpdateBandMemberRoleResponse
{
    public Guid BandId { get; set; }
    public required string TargetUserId { get; set; }
    public BandRole TargetRole { get; set; }
    public required string ActingUserId { get; set; }
    public BandRole ActingRole { get; set; }
}
