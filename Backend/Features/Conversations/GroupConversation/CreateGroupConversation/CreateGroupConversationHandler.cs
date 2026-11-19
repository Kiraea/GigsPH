using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PrjA.Common;
using PrjA.Domain;
using PrjA.Domain.Entities;
using PrjA.Infrastructure;

namespace PrjA.Features.Conversations.GroupConversation.CreateGroupConversation;

public class CreateGroupConversationHandler
{
    private readonly AppDbContext _dbContext;
    private readonly UserManager<AppUser> _userManager;

    public CreateGroupConversationHandler(AppDbContext dbContext, UserManager<AppUser> userManager)
    {
        _dbContext = dbContext;
        _userManager = userManager;
    }

    public async Task<Result<CreateGroupConversationResponse>> HandleAsync(
        CreateGroupConversationRequest request, string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return Result<CreateGroupConversationResponse>.Failure(new Error
            {
                Code = "User.NotFound",
                Message = "User not found",
                Type = ErrorType.NotFound,
            });
        }

        var conversation = new Conversation()
        {
            Type = ConversationType.Group,
            Name = request.Name,
            InitiatedByUserId = userId,
        };

        var conversationMember = new ConversationMember()
        {
            Conversation = conversation,
            UserId = userId,
        };
        

        
        var existingUserIds = await _dbContext.AppUsers.Where(u => request.InvitedUserIds.Contains(u.Id)).Select(u => u.Id).ToListAsync();
        if (request.InvitedUserIds.Except(existingUserIds).Any())
        {
            return Result<CreateGroupConversationResponse>.Failure(new Error
            {
                Code = "Group.InvitedUserNotFound",
                Message = "One or more invited users do not exist.",
                Type = ErrorType.BadRequest,
            });
        }
        
        var invitations = request.InvitedUserIds.Select(id => new GroupChatInvitation()
        {
            InvitedByUserId = userId,
            InvitedToUserId = id,
            Status = GroupChatInvitationStatus.Pending,
            ConversationId = conversation.Id,
        });
        
        
        
        
        _dbContext.Conversations.Add(conversation);
        _dbContext.ConversationMembers.Add(conversationMember);
        _dbContext.GroupChatInvitations.AddRange(invitations);

        await _dbContext.SaveChangesAsync();
        
        
        
        


        
        // TODO: Create a Group conversation and record the acting user as its owner and first member.
        // TODO: Create pending invitations for selected users; do not grant membership before acceptance.
        // TODO: Validate selected accounts, duplicates, and self-invitations. A two-person group is valid.
        // TODO: Save the conversation, owner membership, and invitations together.
        throw new NotImplementedException();
    }
}

