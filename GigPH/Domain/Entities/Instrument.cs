namespace GigPH.Domain;

public class Instrument
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public string Name { get; set;}
    public ICollection<AppUser> AppUsers { get; set; } = [];
}