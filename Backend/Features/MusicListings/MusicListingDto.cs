using PrjA.Domain;
using PrjA.Domain.Entities;
using PrjA.Features.Genres;

namespace PrjA.Features.MusicListings;

public class MusicListingDto
{
    public Guid Id { get; init; }
    public required string Title { get; init; }
    public string? Description { get; init; }
    public  GenreDto? Genre { get; init; }
    public required string City { get; init; }
    public DateTime StartsAt { get; init; }
    public DateTime EndsAt { get; init; }
    public DateTime ExpiresAt { get; init; }
    public IReadOnlyList<MusicListingSlotDto> Slots { get; init; } = [];
}

public class MusicListingSlotDto
{
    public Guid Id { get; init; }
    public Guid InstrumentId { get; init; }
    public bool IsFilled { get; init; }
    public string? Note { get; init; }
}
