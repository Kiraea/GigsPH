using GigPH.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace GigPH.Features.Post.DeletePost;

public class DeletePostHandler
{
    private readonly AppDbContext _dbContext;
    private readonly IS3Service _s3;
    public DeletePostHandler(AppDbContext dbContext, IS3Service s3)
    {
        _dbContext = dbContext;
        _s3 = s3;
    }


    public async Task<DeletePostResponse> HandleAsync(Guid requesterUserId, Guid postId)
    {
        var post = await _dbContext.Posts.Where(p => p.Id == postId && p.UserId == requesterUserId)
            .Include(p => p.Media).FirstOrDefaultAsync();

        if (post == null)
        {
            throw new Exception("post doesnt exist");
        }

        string? key = null;
        if (post.Media != null)
        {
            key = post.Media.Key;
            _dbContext.Medias.Remove(post.Media);
            post.Media = null;
        }

        await _dbContext.SaveChangesAsync();
        if (key != null) await _s3.DeleteAsync(key);
        return new DeletePostResponse();

    }
}