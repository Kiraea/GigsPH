using Microsoft.AspNetCore.Identity;
using PrjA.Common;
using PrjA.Domain.Entities;
using PrjA.Infrastructure;

namespace PrjA.Features.Conversations.GroupConversation.LeaveGroup;

public class LeaveGroupHandler
{
    private readonly AppDbContext _dbContext;
    private readonly UserManager<AppUser> _userManager;

    public LeaveGroupHandler(AppDbContext dbContext, UserManager<AppUser> userManager)
    {
        _dbContext = dbContext;
        _userManager = userManager;
    }

    public async Task<Result<LeaveGroupResponse>> HandleAsync(
        LeaveGroupRequest request, string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return Result<LeaveGroupResponse>.Failure(new Error
            {
                Code = "User.NotFound",
                Message = "User not found",
                Type = ErrorType.NotFound,
            });
        }

        // TODO: Require membership in the requested Group conversation.
        // TODO: Require an owner to transfer ownership before leaving; last-owner/group-deletion behavior remains undecided.
        // TODO: Remove the acting user's membership only; preserve other members and messages.
        throw new NotImplementedException();
    }
}

