using PrjA.Domain;

namespace PrjA.Features.Bands.GetMyBands;

public class GetMyBandResponse
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public BandRole MyRole { get; set; }
    public int MemberCount { get; set; }
    public List<BandMemberDto> Members { get; set; } = [];
    public DateTime CreatedAt { get; set; }
}

public class BandMemberDto
{
    public required string UserId { get; set; }
    public required string DisplayName { get; set; }
    public BandRole Role { get; set; }
    public List<BandMemberInstrumentDto> Instruments { get; set; } = [];
}

public class BandMemberInstrumentDto
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public bool IsMain { get; set; }
}
