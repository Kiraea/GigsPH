namespace PrjA.Features.MusicListings.GetMusicListings;

public class GetMusicListingsResponse
{
    public IReadOnlyList<MusicListingDto> Posts { get; init; } = [];
}
