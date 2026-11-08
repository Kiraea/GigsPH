namespace PrjA.Domain.Entities;

public class UserInstrument
{
    public required string AppUserId { get; set; }
    public AppUser? AppUser { get; set; }
    public required Guid  InstrumentId { get; set; }
    public Instrument? Instrument { get; set; }
    public bool isMain { get; set; }
}