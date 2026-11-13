namespace PrjA.Features.Posts.CreatePost;

public class CreatePostRequest
{
    public List<Guid> UploadIds { get; set; } = [];
    
    public required string Title { get; set; }  
    public required string Description { get; set; }  
    public Guid? BandId { get; set; }
}

