namespace PrjA.Domain.Entities;

public class MusicListing : ITimestamp
{
    public Guid Id { get; set; }
    public string UserId { get; set; }
    public required string Title { get; set; }
    public required string Description { get; set; }
    public required string City { get; set; }
    public Guid? GenreId { get; set; }
    public Genre? Genre { get; set; }
    public DateTime StartsAt { get; init; }
    public DateTime EndsAt { get; init; }
    public DateTime ExpiresAt { get; set; }

    public ICollection<MusicListingSlot> Slots { get; set; } = [];
    public DateTime UpdatedAt { get; set; }
    public DateTime CreatedAt { get; set; }
}
