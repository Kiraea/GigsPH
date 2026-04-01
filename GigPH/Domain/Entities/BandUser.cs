namespace GigPH.Domain;

public class BandUser
{
    public Guid AppUserId { get; set; }
    public Guid BandId { get; set; }
    
    
    public AppUser AppUser { get; set; }
    public Band Band { get; set; }
    
    
}