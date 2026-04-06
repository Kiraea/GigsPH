namespace GigPH.Domain;

public class Band : BaseEntity
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public string Title { get; set; }
    public string Description { get; set; }
    public ICollection<AppUser> AppUsers { get; set; } = [];
    
}