namespace PrjA.Domain.Entities;

public class Band
{
    
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public required string Name { get; set; }
    public string? Description { get; set; }
    
    public ICollection<UserBand>  UserBands { get; set; }

}