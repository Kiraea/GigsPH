using GigPH.Domain;

namespace GigPH.Features.Post.GetMyPosts;

public record GetMyPostsRequest 
{
    public Guid UserId { get; init; }
}