namespace PrjA.Features.Bands.GetPublicBands;

public class GetPublicBandsResponse
{
    public List<PublicBandDto> Bands { get; set; } = [];
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public bool HasMore { get; set; }
}

public class PublicBandDto
{
    public Guid Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public int MemberCount { get; set; }
    public DateTime CreatedAt { get; set; }
}
