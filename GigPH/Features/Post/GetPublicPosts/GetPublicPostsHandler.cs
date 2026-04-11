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
    public async Task<GetPublicPostsWrapperResponse> HandleAsync(int page = 1 , int limit = 5)
    {
        // 5 post in 1 page

        var postToFetch = limit + 1;
        // pages 4 //limit 5 // 15 pages total
        
        // give 5-10
        
        // how do we know that there exists more?
        var posts = await _dbContext.Posts.Include(post => post.Media)
            .Include(p => p.User)
            .Skip((page-1) * limit) 
            .Take(postToFetch)
            .ToListAsync();
        
        
        var hasNextPage = posts.Count == postToFetch;
        
        
        if (hasNextPage)
        {
            posts.RemoveAt(limit);
        }
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
        
        return new GetPublicPostsWrapperResponse()
        {
            Posts = postsResponse ?? [],
            HasNextPage =  hasNextPage
        };

    }
}