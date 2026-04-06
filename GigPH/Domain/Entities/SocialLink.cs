namespace GigPH.Domain;

public class AppUserLink : BaseEntity
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public Guid AppUserId { get; set; } 
    public string Url { get; set; } 
    
    public AppUser AppUser { get; set; }
}