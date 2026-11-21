using PrjA.Domain;

namespace PrjA.Features.MusicListings.CreateMusicListing;

public class CreateMusicListingRequest
{
    public required string Title { get; init; }
    public required string Description { get; init; }
    public Guid? GenreId { get; init; }
    public required string City { get; init; }
    public DateTime StartsAt { get; init; }
    public DateTime EndsAt { get; init; }
    public DateTime ExpiresAt { get; init; }

    public ICollection<MusicListingSlotRequest> MusicListingSlotDto { get; init; } = [];
    
}
public class MusicListingSlotRequest
{
    
    public Guid InstrumentId { get; init; } 
    public string? Note { get; init; } 
}
