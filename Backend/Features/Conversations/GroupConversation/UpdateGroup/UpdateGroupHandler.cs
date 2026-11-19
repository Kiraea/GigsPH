using Microsoft.AspNetCore.Identity;
using PrjA.Common;
using PrjA.Domain.Entities;
using PrjA.Infrastructure;

namespace PrjA.Features.Conversations.GroupConversation.UpdateGroup;

public class UpdateGroupHandler
{
    private readonly AppDbContext _dbContext;
    private readonly UserManager<AppUser> _userManager;

    public UpdateGroupHandler(AppDbContext dbContext, UserManager<AppUser> userManager)
    {
        _dbContext = dbContext;
        _userManager = userManager;
    }

    public async Task<Result<UpdateGroupResponse>> HandleAsync(
        Guid conversationId, UpdateGroupRequest request, string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return Result<UpdateGroupResponse>.Failure(new Error
            {
                Code = "User.NotFound",
                Message = "User not found",
                Type = ErrorType.NotFound,
            });
        }

        // TODO: Require the acting user to own this Group conversation.
        // TODO: Update the group name and return the saved values.
        // TODO: Name length limits and realtime group-detail notifications remain to be designed.
        throw new NotImplementedException();
    }
}

