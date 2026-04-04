using Microsoft.AspNetCore.Identity; 
namespace GigPH.Domain;

public class AppUser : IdentityUser<Guid>
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? DisplayName { get; set; }
    public string? Description { get; set; }
    public bool IsOnboarded { get; set; } = false;

    public ICollection<BandUser> BandUsers { get; set; }= [];
    public ICollection<AppUserLink> AppUserLinks { get; set; } = [];
}