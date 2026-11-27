namespace PrjA.Domain.Entities;

public class Location : ITimestamp
{

    public Guid Id { get; set; } = Guid.CreateVersion7();
    public required string Country { get; set; }
    public required string ProvinceState { get; set; }
    public required string City { get; set; }
    
    
    public double? latitude { get; set; }
    public double? longitude { get; set; }
    
    public ICollection<AppUser> AppUsers { get; set; } = new List<AppUser>();


    public DateTime UpdatedAt { get; set; }
    public DateTime CreatedAt { get; set; }
}
