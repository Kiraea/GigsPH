using PrjA.Common;
using PrjA.Infrastructure;

namespace PrjA.Features.Posts.GetPublicPosts;

public class GetPublicPostsHandler
{
    private readonly AppDbContext _dbContext;

    public GetPublicPostsHandler(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<GetPublicPostsResponse>> HandleAsync(
        GetPublicPostsRequest request)
    {
        // TODO: Query visible public posts with AsNoTracking, stable ordering, pagination, and DTO projection.
        throw new NotImplementedException();
    }
}

