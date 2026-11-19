using Microsoft.AspNetCore.Identity;
using PrjA.Common;
using PrjA.Domain.Entities;
using PrjA.Infrastructure;

namespace PrjA.Features.Conversations.GroupConversation.TransferGroupOwnership;

public class TransferGroupOwnershipHandler
{
    private readonly AppDbContext _dbContext;
    private readonly UserManager<AppUser> _userManager;

    public TransferGroupOwnershipHandler(AppDbContext dbContext, UserManager<AppUser> userManager)
    {
        _dbContext = dbContext;
        _userManager = userManager;
    }

    public async Task<Result<TransferGroupOwnershipResponse>> HandleAsync(
        Guid conversationId, TransferGroupOwnershipRequest request, string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return Result<TransferGroupOwnershipResponse>.Failure(new Error
            {
                Code = "User.NotFound",
                Message = "User not found",
                Type = ErrorType.NotFound,
            });
        }

        // TODO: Require the acting user to be the current owner of this Group conversation.
        // TODO: Require a different existing accepted member as the new owner; pending invitees cannot own the group.
        // TODO: Transfer ownership without changing either user's conversation membership.
        throw new NotImplementedException();
    }
}

