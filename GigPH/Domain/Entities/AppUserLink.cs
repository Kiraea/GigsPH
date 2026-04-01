namespace GigPH.Domain;

public class AppUserLink
{
    public Guid Id { get; set; } 
    public Guid AppUserId { get; set; } 
    public string Url { get; set; } 
    
    public AppUser AppUser { get; set; }
}