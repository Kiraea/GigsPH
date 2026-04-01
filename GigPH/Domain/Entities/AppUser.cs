using Microsoft.AspNetCore.Identity;

namespace GigPH.Domain;

public class AppUser : IdentityUser<Guid>
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public bool isVerified { get; set; } = false;

    public ICollection<BandUser> BandUsers { get; set; }= [];
}