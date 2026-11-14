namespace PrjA.Domain.Entities;

public class UserScoreInstrument 
{
    
    public required string Userid { get; set; }
    public required Guid InstrumentId { get; set; }
    public required int Score { get; set; }
}