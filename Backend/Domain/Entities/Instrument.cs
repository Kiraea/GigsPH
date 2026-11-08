namespace PrjA.Domain.Entities;

public class Instrument
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public required string Name { get; set; }

    public ICollection<UserInstrument> UserInstruments { get; set; } = [];
}