using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PrjA.Common;
using PrjA.Domain.Entities;
using PrjA.Infrastructure;

namespace PrjA.Features.Posts.SetLikeStatusPost;

public class SetLikeStatusPostHandler
{
    private readonly AppDbContext _dbContext;
    private readonly UserManager<AppUser> _userManager;

    public SetLikeStatusPostHandler(AppDbContext dbContext, UserManager<AppUser> userManager)
    {
        _dbContext = dbContext;
        _userManager = userManager;
    }

    public async Task<Result<SetLikeStatusPostResponse>> HandleAsync(
        Guid postId, SetLikeStatusPostRequest request, string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return Result<SetLikeStatusPostResponse>.Failure(new Error
            {
                Code = "User.NotFound",
                Message = "User not found",
                Type = ErrorType.NotFound,
            });
        }
        
        var post =  await _dbContext.Posts.FindAsync(postId);

        if (post == null)
        {
                
            return Result<SetLikeStatusPostResponse>.Failure(new Error
            {
                Code = "Post.NotFound",
                Message = "Post not found",
                Type = ErrorType.NotFound,
            });
        }
        
        var postLike = await  _dbContext.PostLikes.FirstOrDefaultAsync(pl => pl.PostId == postId &&  pl.UserId == userId);
        

        
        
        if (request.IsLiked == true && postLike == null)
        {
            _dbContext.PostLikes.Add(new PostLike
            {
                PostId = postId,
                UserId = userId,
            });
        }

        if (request.IsLiked == false && postLike != null)
        {
            _dbContext.PostLikes.Remove(postLike);   
        }


        
        await _dbContext.SaveChangesAsync();

        return Result<SetLikeStatusPostResponse>.Success(new SetLikeStatusPostResponse()
        {
            isLiked = request.IsLiked,
            postId = postId,
        });
        

    }
}

