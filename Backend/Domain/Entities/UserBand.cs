namespace PrjA.Domain.Entities;

public class UserBand
{
    public required string UserId { get; set; }
    public AppUser?  User{ get; set; }

    public Guid BandId { get; set; }
    public Band? Band { get; set; }
    
    public BandRole Role { get; set; }
    
    
    
    }