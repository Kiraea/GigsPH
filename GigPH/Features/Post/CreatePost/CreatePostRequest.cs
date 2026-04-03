namespace GigPH.Features.Post.CreatePost;


public record CreatePostRequest
{
    
    public Guid UserId { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public IFormFile? File { get; set; }

}


/*
    public Guid UserId { get; set; }
    public string Title { get; set; }
    public string description { get; set; }
    public IFormFile? File { get; set; }
*/