using Microsoft.AspNetCore.Identity;

namespace PrjA.Domain.Entities;

public class AppUser : IdentityUser
{
    public string? Firstname { get; set; }
    public string? LastName { get; set; }
    public string? DisplayName { get; set; }
    public string? Description { get; set; }

    public bool IsOnboarded { get; set; } = false;
    public ICollection<Genre> Genres { get; set; } = new List<Genre>();
    public ICollection<Instrument> Instruments { get; set; } = [];
    public ICollection<SocialLink> SocialLinks { get; set; } = [];
    public ICollection<Band> Band { get; set; } = [];
    
    
    public Guid? LocationId { get; set; } 
    public Location? Location{ get; set; }
}