namespace PrjA.Domain.Entities;

public class PostLikes : ITimestamp
{
    public Guid PostId { get; set; }
    public Post? Post { get; set; }
    public string UserId { get; set; }
    public AppUser? User { get; set; }

    public DateTime UpdatedAt { get; set; }
    public DateTime CreatedAt { get; set; }
}