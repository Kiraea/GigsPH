using Microsoft.AspNetCore.Identity;
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

        // TODO: Check post visibility and set the acting user's like to IsLiked; repeated requests must keep the same state.
        throw new NotImplementedException();
    }
}

