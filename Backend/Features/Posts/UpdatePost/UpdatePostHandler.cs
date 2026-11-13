using Microsoft.AspNetCore.Identity;
using PrjA.Common;
using PrjA.Domain.Entities;
using PrjA.Infrastructure;

namespace PrjA.Features.Posts.UpdatePost;

public class UpdatePostHandler
{
    private readonly AppDbContext _dbContext;
    private readonly UserManager<AppUser> _userManager;

    public UpdatePostHandler(AppDbContext dbContext, UserManager<AppUser> userManager)
    {
        _dbContext = dbContext;
        _userManager = userManager;
    }

    public async Task<Result<UpdatePostResponse>> HandleAsync(
        Guid postId, UpdatePostRequest request, string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return Result<UpdatePostResponse>.Failure(new Error
            {
                Code = "User.NotFound",
                Message = "User not found",
                Type = ErrorType.NotFound,
            });
        }

        // TODO: Find the post, check ownership, and apply the supplied changes.
        throw new NotImplementedException();
    }
}

