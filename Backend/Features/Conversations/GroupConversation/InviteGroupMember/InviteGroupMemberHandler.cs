using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PrjA.Common;
using PrjA.Domain;
using PrjA.Domain.Entities;
using PrjA.Infrastructure;

namespace PrjA.Features.Conversations.GroupConversation.InviteGroupMember;

public class InviteGroupMemberHandler
{
    private readonly AppDbContext _dbContext;
    private readonly UserManager<AppUser> _userManager;

    public InviteGroupMemberHandler(AppDbContext dbContext, UserManager<AppUser> userManager)
    {
        _dbContext = dbContext;
        _userManager = userManager;
    }

    public async Task<Result<InviteGroupMemberResponse>> HandleAsync(
        Guid conversationId, InviteGroupMemberRequest request, string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return Result<InviteGroupMemberResponse>.Failure(new Error
            {
                Code = "User.NotFound",
                Message = "User not found",
                Type = ErrorType.NotFound,
            });
        }
        
        // check if oiwner part of conversation
        // check if the user is currently a part of the conversation 
        // check if the one being invited is has an invitation already and if that invitation is pending -> if it is reject -> if accept still continue cause we already check if he part of convo
        // check if user is inviting himseelf

        if (userId == request.UserId)
        {
            
            return Result<InviteGroupMemberResponse>.Failure(new Error
            {
                Code = "GroupChatInvitation.CannotInviteSelf",
                Message = "Cannot invite self in group chat",
                Type = ErrorType.BadRequest,
            });
        }
  
        
        var conversationMember = await _dbContext.ConversationMembers.AnyAsync(cm => 
            cm.Conversation.Id == conversationId &&
            cm.UserId == userId
            && cm.Conversation.OwnerUserId == userId
            // check for group cause it might be a direct chatId
            && cm.Conversation.Type == ConversationType.Group
        );

        if (!conversationMember)
        {
            
            return Result<InviteGroupMemberResponse>.Failure(new Error
            {
                Code = "Conversation.Unauthorized",
                Message = "User not part of conversation",
                Type = ErrorType.Forbidden,
            });
        }
        
        var inviteeExists = await _dbContext.AppUsers
            .AnyAsync(u => u.Id == request.UserId);

        if (!inviteeExists)
        {
            
            return Result<InviteGroupMemberResponse>.Failure(new Error
            {
                Code = "User.NotExist",
                Message = "Invited User does not exist",
                Type = ErrorType.NotFound,
            });
        }
        
        var isInConversation = await _dbContext.ConversationMembers.AnyAsync(cm => cm.Conversation.Id == conversationId && cm.UserId== request.UserId);

        if (isInConversation)
        {
            return Result<InviteGroupMemberResponse>.Failure(new Error
            {
                Code = "Conversation.AlreadyInConversation",
                Message = "User already in conversation",
                Type = ErrorType.BadRequest,
            });
            
        }
        var hasPendingInvite = await _dbContext.GroupChatInvitations.FirstOrDefaultAsync(gci =>
            gci.InvitedToUserId == request.UserId 
            && gci.Status == GroupChatInvitationStatus.Pending
            && gci.ConversationId == conversationId);
        if (hasPendingInvite != null)
        {
            
            return Result<InviteGroupMemberResponse>.Failure(new Error
            {
                Code = "GroupChatInvitation.AlreadyInvited",
                Message = "User already invited in group chat",
                Type = ErrorType.BadRequest,
            });
        }
        var invitation = new GroupChatInvitation()
        {
            InvitedToUserId = request.UserId,
            InvitedByUserId = userId,
            Status = GroupChatInvitationStatus.Pending,
            ConversationId = conversationId,
        };
        _dbContext.GroupChatInvitations.Add(invitation);
        
        await _dbContext.SaveChangesAsync();


        return Result<InviteGroupMemberResponse>.Success(new InviteGroupMemberResponse()
        {
            ConversationId = conversationId,
            InvitationId = invitation.id,
        });
    }
}

