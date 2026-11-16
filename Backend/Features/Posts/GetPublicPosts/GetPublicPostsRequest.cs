namespace PrjA.Features.Posts.GetPublicPosts;

public class GetPublicPostsRequest
{
    public string? Cursor { get; set; } 
    public int PageSize { get; set; } = 20;
}

