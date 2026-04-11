namespace GigPH.Features.Post.GetPublicPosts;

public record GetPublicPostsResponse
{
    public Guid Id { get; init; }
    public Guid UserId { get; init; }
    public string DisplayName { get; init; }
    public DateTime CreatedAt { get; init; }
    public string Title { get; init; }
    public string Description { get; init; }
    public string? MediaUrl { get; init; }
    public string? MediaType { get; init; }
    public string? FileName { get; init; }  
    
 
}