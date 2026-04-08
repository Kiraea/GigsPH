namespace GigPH.Domain;

public class Location
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    
    
    public string Country { get; set; }
    public string City { get; set; }
    public string ProvinceState { get; set; }

    public ICollection<AppUser> AppUsers { get; set; } = [];

}
