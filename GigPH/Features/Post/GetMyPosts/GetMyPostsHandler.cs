using GigPH.Features.Post.GetMyPost;
using GigPH.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace GigPH.Features.Post.GetMyPosts;

public class GetMyPostsHandler
{
    private readonly AppDbContext _dbContext; 
    private readonly IS3Service _s3; 

    public GetMyPostsHandler(IS3Service s3, AppDbContext dbContext)
    {
        _dbContext = dbContext;
        _s3 = s3;
    }

    public async Task<List<GetMyPostsResponse>> HandleAsync(GetMyPostsRequest request)
    {
        var postList = await _dbContext.Posts
            .Where(p => p.UserId == request.UserId)
            .Include(p => p.Media)
            .ToListAsync();
        
        return postList.Select(p => new GetMyPostsResponse
        {
            Description = p.Description,
            FileName = p.Media?.Name,
            Id = p.Id,
            UserId = p.UserId,
            Title = p.Title,
            MediaType = p.Media?.Type,
            MediaUrl = p.Media!= null ? _s3.GetPresignedUrl(p.Media.Key) : null
        }).ToList();

    }
}