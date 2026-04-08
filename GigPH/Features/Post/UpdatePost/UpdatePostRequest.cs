namespace GigPH.Features.Post.UpdatePost;

public class UpdatePostRequest
{
    
    public Guid UserId { get; set; }
    public Guid? PostId { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public IFormFile? File { get; set; }
    public bool RemoveMedia { get; set; }

}