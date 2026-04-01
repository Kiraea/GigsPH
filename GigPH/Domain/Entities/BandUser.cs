namespace GigPH.Domain;

public class BandUser
{
    public int AppUserId { get; set; }
    public int BandId { get; set; }
    
    
    public AppUser AppUser { get; set; }
    public Band Band { get; set; }
    
    
}