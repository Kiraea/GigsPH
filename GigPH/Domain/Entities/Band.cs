namespace GigPH.Domain;

public class Band
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public string title { get; set; }
    public string description { get; set; }


    public ICollection<BandUser> BandUsers { get; set; } = [];
    
}