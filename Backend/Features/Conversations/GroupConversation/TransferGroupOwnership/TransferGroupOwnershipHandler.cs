using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PrjA.Common;
using PrjA.Domain;
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

        
        // check if user owner
        // check if selected user exist
        // transfer
        // cannot transfer to oneself


        var conversation = await _dbContext.Conversations.Where(c =>
            c.Id == conversationId &&
            c.Type == ConversationType.Group &&
            c.ConversationMembers.Any(cm => user.Id == cm.UserId)).FirstOrDefaultAsync();

        if (conversation == null)
        {
            return Result<TransferGroupOwnershipResponse>.Failure(new Error
            {
                Code = "Conversation.NotFound",
                Message = "Group conversation not found.",
                Type = ErrorType.NotFound,
            });
        }
        
        if (conversation.OwnerUserId != userId)
        {
            return Result<TransferGroupOwnershipResponse>.Failure(new Error
            {
                Code = "Conversation.Forbidden",
                Message = "Only the group owner can transfer ownership.",
                Type = ErrorType.Forbidden,
            });
        }
        if (request.NewOwnerUserId == userId)
        {
            return Result<TransferGroupOwnershipResponse>.Failure(new Error
            {
                Code = "Conversation.AlreadyOwner",
                Message = "You already own this group.",
                Type = ErrorType.BadRequest,
            });
        }
        
        var newOwnerIsMember = await _dbContext.ConversationMembers.AnyAsync(cm =>
            cm.ConversationId == conversation.Id &&
            cm.UserId == request.NewOwnerUserId);

        if (!newOwnerIsMember)
        {
            return Result<TransferGroupOwnershipResponse>.Failure(new Error
            {
                Code = "Conversation.MemberNotFound",
                Message = "The new owner must be a member of this group.",
                Type = ErrorType.NotFound,
            });
        }

        conversation.OwnerUserId = request.NewOwnerUserId;
        await _dbContext.SaveChangesAsync();

        return Result<TransferGroupOwnershipResponse>.Success(
            new TransferGroupOwnershipResponse
            {
                ConversationId = conversation.Id,
                OwnerUserId = request.NewOwnerUserId,
            });


    }
}

