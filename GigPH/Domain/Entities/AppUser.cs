using Microsoft.AspNetCore.Identity; 
namespace GigPH.Domain;

public class AppUser : IdentityUser<Guid>
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? DisplayName { get; set; }
    public string? Description { get; set; }
    public bool IsOnboarded { get; set; } = false;

    public ICollection<Band> Bands { get; set; }= [];
    public ICollection<SocialLink> SocialLinks { get; set; } = [];
    public ICollection<Post> Posts { get; set; } = [];
    public ICollection<Genre> Genres { get; set; } = [];
    public ICollection<Instrument> Instruments{ get; set; } = [];
    
}