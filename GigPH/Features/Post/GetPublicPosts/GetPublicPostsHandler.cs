using GigPH.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace GigPH.Features.Post.GetPublicPosts;

public class GetPublicPostsHandler
{

    private readonly AppDbContext _dbContext;
    private readonly IS3Service _s3;
    public GetPublicPostsHandler(AppDbContext dbContext, IS3Service s3)
    {
        _dbContext = dbContext;
        _s3 = s3;

    }
    public async Task<List<GetPublicPostsResponse>> HandleAsync()
    {
        var posts = await _dbContext.Posts.Include(post => post.Media)
            .Include(p => p.User)
            .ToListAsync();
        var postsResponse = posts.Select(p => new GetPublicPostsResponse()
        {
            Description = p.Description,
            Id = p.Id,
            DisplayName = p.User.DisplayName!,
            CreatedAt = p.CreatedAt,
            MediaType = p.Media?.Type,
            FileName = p.Media?.Name,
            MediaUrl = p.Media != null ? _s3.GetPresignedUrl(p.Media.Key) : null,
            UserId = p.UserId,
            Title = p.Title,
        }).ToList();
        return postsResponse;

    }
}