namespace PrjA.Domain.Entities;

public class Band : ITimestamp
{
    
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public required string Name { get; set; }
    public string? Description { get; set; }
    
    public ICollection<UserBand>  UserBands { get; set; }

    public DateTime UpdatedAt { get; set; }
    public DateTime CreatedAt { get; set; }
}