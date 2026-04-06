namespace GigPH.Domain;

public class Genre
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public string Name { get; set; }

    public ICollection<AppUser> AppUsers { get; set; } = [];
}