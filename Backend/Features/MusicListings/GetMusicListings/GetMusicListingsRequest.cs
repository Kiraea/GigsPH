using PrjA.Domain;

namespace PrjA.Features.MusicListings.GetMusicListings;

public class GetMusicListingsRequest
{
    public Guid? GenreId { get; init; }
    public Guid? InstrumentId { get; init; }
    public string? City { get; init; }
    public DateTime? StartsAfter { get; init; }
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
}
