namespace GigPH.Domain;

public class AppUser
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public bool isVerified { get; set; } = false;

    public ICollection<BandUser> BandUsers = [];
}