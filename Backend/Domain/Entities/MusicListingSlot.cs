namespace PrjA.Domain.Entities;

public class MusicListingSlot : ITimestamp
{
    public Guid Id { get; set; }
    
    public Guid MusicListingId { get; set; }
    public MusicListing? MusicListing { get; set; }
    
    public Guid InstrumentId { get; set; }
    public Instrument? Instrument { get; set; }
    public string? Note { get; set; }
    

    public ICollection<MusicListingApplication> Applications { get; set; } = [];


    public DateTime UpdatedAt { get; set; }
    public DateTime CreatedAt { get; set; }
}