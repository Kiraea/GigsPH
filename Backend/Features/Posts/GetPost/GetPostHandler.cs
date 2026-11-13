using PrjA.Common;
using PrjA.Infrastructure;

namespace PrjA.Features.Posts.GetPost;

public class GetPostHandler
{
    private readonly AppDbContext _dbContext;

    public GetPostHandler(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<GetPostResponse>> HandleAsync(
        GetPostRequest request)
    {
        // TODO: Query a visible post with AsNoTracking and project to a public DTO; return NotFound if unavailable.
        throw new NotImplementedException();
    }
}

