using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PrjA.Common;
using PrjA.Domain;
using PrjA.Domain.Entities;
using PrjA.Infrastructure;

namespace PrjA.Features.Conversations.GroupConversation.RespondToGroupInvitation;

public class RespondToGroupInvitationHandler
{
    private readonly AppDbContext _dbContext;
    private readonly UserManager<AppUser> _userManager;

    public RespondToGroupInvitationHandler(AppDbContext dbContext, UserManager<AppUser> userManager)
    {
        _dbContext = dbContext;
        _userManager = userManager;
    }

    public async Task<Result<RespondToGroupInvitationResponse>> HandleAsync(
        Guid invitationId, RespondToGroupInvitationRequest request, string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return Result<RespondToGroupInvitationResponse>.Failure(new Error
            {
                Code = "User.NotFound",
                Message = "User not found",
                Type = ErrorType.NotFound,
            });
        }
        
        
        var invitation = await  _dbContext.GroupChatInvitations.FirstOrDefaultAsync(
            gci => gci.Id == invitationId && gci.InvitedToUserId == userId && gci.Status == GroupChatInvitationStatus.Pending);
        if (invitation == null)
        {
            
            return Result<RespondToGroupInvitationResponse>.Failure(new Error
            {
                Code = "Invitation.NotFound",
                Message = "Invitation not found",
                Type = ErrorType.NotFound,
            });
        }

        if (request.Accept)
        {
            var groupExists = await _dbContext.Conversations.AnyAsync(c =>
                c.Id == invitation.ConversationId &&
                c.Type == ConversationType.Group);

            if (!groupExists)
            {
                return Result<RespondToGroupInvitationResponse>.Failure(new Error
                {
                    Code = "Conversation.NotFound",
                    Message = "The group conversation was not found.",
                    Type = ErrorType.NotFound,
                });
            }

            var alreadyMember = await _dbContext.ConversationMembers.AnyAsync(cm =>
                cm.ConversationId == invitation.ConversationId &&
                cm.UserId == userId);

            if (!alreadyMember)
            {
                _dbContext.ConversationMembers.Add(new ConversationMember
                {
                    ConversationId = invitation.ConversationId,
                    UserId = userId,
                });
            }

            invitation.Status = GroupChatInvitationStatus.Accepted;
        }
        else
        {
            invitation.Status = GroupChatInvitationStatus.Declined;
        }

        await _dbContext.SaveChangesAsync();

        return Result<RespondToGroupInvitationResponse>.Success(
            new RespondToGroupInvitationResponse
            {
                InvitationId = invitation.Id,
                ConversationId = request.Accept ? invitation.ConversationId : null,
                Status = invitation.Status,
            });


    }
}

