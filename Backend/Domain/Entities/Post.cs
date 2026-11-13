namespace PrjA.Domain.Entities;

public class Post
{
    public Guid Id { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }

    public ICollection<PostLikes> Likes { get; set; } = [];
    public ICollection<MediaUpload> MediaUploads { get; set; } = [];
    
    
    
    // can only be either 
    public string? UserId { get; set; }
    public Guid? BandId { get; set; }
    public required string CreatedByUserId { get; set; }
}