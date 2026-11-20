using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PrjA.Common;
using PrjA.Domain;
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


        if (request.MemberUserId == user.Id)
        {
            
            return Result<RemoveGroupMemberResponse>.Failure(new Error
            {
                Code = "User.CannotRemoveSelf",
                Message = "Transfer ownership before leaving the group.",
                Type = ErrorType.BadRequest,
            });
        }

        var hasPermission = await _dbContext.ConversationMembers.AnyAsync(cm =>
            cm.ConversationId == request.ConversationId && cm.Conversation.OwnerUserId == userId &&
            cm.UserId == userId && cm.Conversation.Type == ConversationType.Group);

        if (!hasPermission)
        {
            return Result<RemoveGroupMemberResponse>.Failure(new Error
            {
                Code = "Conversation.CannotRemove",
                Message = "User Does not have permission to remove member",
                Type = ErrorType.Forbidden,
            });

        }

        var conversationMember =
            await _dbContext.ConversationMembers.FirstOrDefaultAsync(cm => request.MemberUserId == cm.UserId && request.ConversationId == cm.ConversationId);
        if (conversationMember == null)
        {
            return Result<RemoveGroupMemberResponse>.Failure(new Error
            {
                Code = "Conversation.User not in group",
                Message = "Cannot Remove member as user does not exist",
                Type = ErrorType.NotFound,
            });
        }
        
        
        _dbContext.ConversationMembers.Remove(conversationMember);
        await _dbContext.SaveChangesAsync();
        return Result<RemoveGroupMemberResponse>.Success(
            new RemoveGroupMemberResponse
            {
                ConversationId = conversationMember.ConversationId,
                RemovedUserId = conversationMember.UserId,
            });

    }
}

