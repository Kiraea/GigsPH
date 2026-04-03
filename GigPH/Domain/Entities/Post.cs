namespace GigPH.Domain;

public class Post
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    
    public Guid? MediaId { get; set; }
    public Media? Media { get; set; }
    
}