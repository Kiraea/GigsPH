namespace PrjA.Domain.Entities;

public class Genre : ITimestamp
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public required string Name { get; set; }
    
    
    public ICollection<AppUser> AppUsers { get; set; } = new List<AppUser>();

    public DateTime UpdatedAt { get; set; }
    public DateTime CreatedAt { get; set; }
}