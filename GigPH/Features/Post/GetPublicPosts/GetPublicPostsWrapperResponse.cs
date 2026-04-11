namespace GigPH.Features.Post.GetPublicPosts;

public record GetPublicPostsWrapperResponse
{
    public List<GetPublicPostsResponse> Posts { get; init; } = [];
    public bool HasNextPage { get; init; }
}