namespace PrjA.Domain.Entities;

public class PostLikes
{
    public Guid PostId { get; set; }
    public Post? Post { get; set; }
    public string UserId { get; set; }
    public AppUser? User { get; set; }
    
}