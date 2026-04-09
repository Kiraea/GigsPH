namespace GigPH.Features.Post.UpdatePost;

public class UpdatePostRequest
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public IFormFile? File { get; set; }
    public bool RemoveMedia { get; set; }

}