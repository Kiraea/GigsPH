namespace PrjA.Features.Posts.GetPublicPosts;

public class GetPublicPostsRequest
{
    public string? Cursor { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

