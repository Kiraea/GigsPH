namespace GigPH.Domain;

public class SocialLink : BaseEntity
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public Guid AppUserId { get; set; } 
    
    public AppUser AppUser { get; set; }
    public string Url { get; set; } 
}