namespace PrjA.Domain.Entities;

public class Instrument :ITimestamp
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public required string Name { get; set; }

    public ICollection<UserInstrument> UserInstruments { get; set; } = [];
    public DateTime UpdatedAt { get; set; }
    public DateTime CreatedAt { get; set; }
}