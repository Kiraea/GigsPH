using GigPH.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace GigPH.Features.Post.GetPublicPost;

public class GetPublicPostHandler
{
    private readonly AppDbContext _dbContext;
    private readonly IS3Service _s3;
    public GetPublicPostHandler(AppDbContext dbContext, IS3Service s3)
    {
        _dbContext = dbContext;
        _s3 = s3;

    }
    public async Task<GetPublicPostResponse> HandleAsync(Guid postId)
    {
        var post = await _dbContext.Posts.Include(post => post.Media)
            .Include(p => p.User).FirstOrDefaultAsync(p => p.Id == postId);

        if (post == null)
        {
            throw new Exception("post not exist");
        }

        var response = new GetPublicPostResponse()
        {
            Description = post.Description,
            Id = post.Id,
            DisplayName = post.User.DisplayName!,
            CreatedAt = post.CreatedAt,
            MediaType = post.Media?.Type,
            FileName = post.Media?.Name,
            MediaUrl = post.Media != null ? _s3.GetPresignedUrl(post.Media.Key) : null,
            UserId = post.UserId,
            Title = post.Title,
        };
        return response;

    }
}
