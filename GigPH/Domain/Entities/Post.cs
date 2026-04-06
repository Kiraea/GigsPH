namespace GigPH.Domain;

public class Post :BaseEntity
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public Guid UserId { get; set; }
    public AppUser User { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    
    public Guid? MediaId { get; set; }
    public Media? Media { get; set; }
    
    
    
}