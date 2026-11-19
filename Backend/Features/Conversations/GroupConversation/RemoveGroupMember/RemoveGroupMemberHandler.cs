using Microsoft.AspNetCore.Identity;
using PrjA.Common;
using PrjA.Domain.Entities;
using PrjA.Infrastructure;

namespace PrjA.Features.Conversations.GroupConversation.RemoveGroupMember;

public class RemoveGroupMemberHandler
{
    private readonly AppDbContext _dbContext;
    private readonly UserManager<AppUser> _userManager;

    public RemoveGroupMemberHandler(AppDbContext dbContext, UserManager<AppUser> userManager)
    {
        _dbContext = dbContext;
        _userManager = userManager;
    }

    public async Task<Result<RemoveGroupMemberResponse>> HandleAsync(
        RemoveGroupMemberRequest request, string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return Result<RemoveGroupMemberResponse>.Failure(new Error
            {
                Code = "User.NotFound",
                Message = "User not found",
                Type = ErrorType.NotFound,
            });
        }

        // TODO: Require the acting user to own this Group conversation.
        // TODO: Remove only the target membership, preserving the conversation and messages.
        // TODO: Use LeaveGroup for leaving yourself and TransferGroupOwnership before an owner leaves.
        // TODO: Removed users must no longer pass membership checks or receive future group-message events.
        throw new NotImplementedException();
    }
}

