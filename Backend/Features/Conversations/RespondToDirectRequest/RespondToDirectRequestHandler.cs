using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using PrjA.Common;
using PrjA.Domain;
using PrjA.Domain.Entities;
using PrjA.Features.Conversations.Realtime;
using PrjA.Infrastructure;

namespace PrjA.Features.Conversations.RespondToDirectRequest;

public class RespondToDirectRequestHandler
{
    private readonly AppDbContext _dbContext;
    private readonly IHubContext<Chathub, IChatClient> _chathub;
    private readonly ILogger<RespondToDirectRequestHandler> _logger;
    private readonly UserManager<AppUser> _userManager;

    public RespondToDirectRequestHandler(
        AppDbContext dbContext,
        UserManager<AppUser> userManager,
        IHubContext<Chathub, IChatClient> chatHub,
        ILogger<RespondToDirectRequestHandler> logger)
    {
        _dbContext = dbContext;
        _userManager = userManager;
        _chathub = chatHub;
        _logger = logger;
    }

    public async Task<Result<RespondToDirectRequestResponse>> HandleAsync(
        Guid conversationId, string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return Result<RespondToDirectRequestResponse>.Failure(new Error
            {
                Code = "User.NotFound",
                Message = "User not found",
                Type = ErrorType.NotFound,
            });
        }

        var conversation = await _dbContext.ConversationMembers
            .Where(x => x.ConversationId == conversationId && x.UserId == user.Id)
            .Select(cm => cm.Conversation)
            .FirstOrDefaultAsync();

        if (conversation == null)
        {
            return Result<RespondToDirectRequestResponse>.Failure(new Error
            {
                Code = "Conversation.NotFound",
                Message = "Conversation not found",
                Type = ErrorType.NotFound,
            });
        }

        if (conversation.Type != ConversationType.Direct)
        {
            return Result<RespondToDirectRequestResponse>.Failure(new Error
            {
                Code = "Conversation.NotDirectMessage",
                Message = "Conversation not direct message",
                Type = ErrorType.BadRequest,
            });
        }

        if (conversation.InitiatedByUserId == userId)
        {
            return Result<RespondToDirectRequestResponse>.Failure(new Error
            {
                Code = "Conversation.RecipientRequired",
                Message = "Only the recipient can accept this request",
                Type = ErrorType.BadRequest,
            });
        }

        var isBlocked = await _dbContext.UserBlocks.AnyAsync(b =>
            (b.BlockerUserId == user.Id &&
             b.BlockedUserId == conversation.InitiatedByUserId) ||
            (b.BlockerUserId == conversation.InitiatedByUserId &&
             b.BlockedUserId == user.Id));

        if (isBlocked)
        {
            return Result<RespondToDirectRequestResponse>.Failure(new Error
            {
                Code = "Conversation.ContactBlocked",
                Message = "The request cannot be accepted while contact is blocked.",
                Type = ErrorType.Forbidden,
            });
        }

        if (conversation.DirectRequestStatus != DirectRequestStatus.Pending)
        {
            return Result<RespondToDirectRequestResponse>.Failure(new Error
            {
                Code = "Conversation.NotPending",
                Message = "Conversation not Pending message",
                Type = ErrorType.BadRequest,
            });
        }

        conversation.DirectRequestStatus = DirectRequestStatus.Accepted;

        await _dbContext.SaveChangesAsync();

        try
        {
            var participantIds = await _dbContext.ConversationMembers
                .Where(cm => cm.ConversationId == conversation.Id)
                .Select(cm => cm.UserId)
                .ToListAsync();

            await _chathub.Clients.Users(participantIds).ConversationAccepted(conversation.Id);
        }
        catch (Exception exception)
        {
            _logger.LogWarning(exception, "Acceptance notification failed for conversation");
        }

        return Result<RespondToDirectRequestResponse>.Success(new RespondToDirectRequestResponse()
        {
            ConversationId = conversationId,
        });
    }
}
