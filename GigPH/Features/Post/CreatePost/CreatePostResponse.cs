namespace GigPH.Features.Post.CreatePost;


public record CreatePostResponse
{
    
    public Guid Id { get; init; }
    public Guid UserId { get; init; }
    public string Title { get; init; }
    public string Description { get; init; }
    public string? Url { get; init; }
    public string? MediaType { get; init; }
    public string? FileName { get; init; }
    
}
