using System.Reflection.Metadata.Ecma335;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using PrjA.Common;
using PrjA.Domain;
using PrjA.Domain.Entities;
using PrjA.Features.Conversations.Realtime;
using PrjA.Infrastructure;

namespace PrjA.Features.Conversations.SendMessage;

public class SendMessageHandler
{
    private readonly AppDbContext _dbContext;
    private readonly UserManager<AppUser> _userManager;
    private readonly IHubContext<Chathub, IChatClient> _chathub;
    private readonly ILogger<SendMessageHandler> _logger;

    public SendMessageHandler(AppDbContext dbContext, UserManager<AppUser> userManager, IHubContext<Chathub, IChatClient> chathub,  ILogger<SendMessageHandler> logger)
    {
        _dbContext = dbContext;
        _userManager = userManager;
        _chathub = chathub;
        _logger = logger;
    }

    public async Task<Result<SendMessageResponse>> HandleAsync(
        Guid conversationId, SendMessageRequest request, string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return Result<SendMessageResponse>.Failure(new Error
            {
                Code = "User.NotFound",
                Message = "User not found",
                Type = ErrorType.NotFound,
            });
        }
        
        var conversation = await _dbContext.ConversationMembers
            .Where(cm => cm.UserId == userId &&
                         cm.ConversationId == conversationId)
            .Select(cm => cm.Conversation)
            .FirstOrDefaultAsync();
        if (conversation == null)
        {
            return Result<SendMessageResponse>.Failure(new Error
            {
                Code = "Conversation.NotFound",
                Message = "Conversation not found",
                Type = ErrorType.NotFound,
            });
        }
        
        var isDuplicate = await _dbContext.Messages
            .AnyAsync(m => m.ClientMessageId == request.ClientMessageId);


        if (isDuplicate)
        {
            
            return Result<SendMessageResponse>.Failure(new Error
            {
                Code = "Conversation.DuplicateMessage",
                Message = "Message already Exist",
                Type = ErrorType.BadRequest,
            });
        }

        if (conversation.Type == ConversationType.Direct)
        {
            

            var otherUserId = await _dbContext.ConversationMembers
                .Where(cm => cm.ConversationId == conversationId && cm.UserId != userId).Select(cm => cm.UserId).SingleAsync();
                
            var isBlocked = await _dbContext.UserBlocks.AnyAsync(b =>
                (b.BlockerUserId == userId && b.BlockedUserId == otherUserId) ||
                (b.BlockerUserId == otherUserId && b.BlockedUserId == userId));
            if (isBlocked)
            {
                return Result<SendMessageResponse>.Failure(new Error
                {
                    Code = "Conversation.ContactBlocked",
                    Message = "Messaging is unavailable between these accounts.",
                    Type = ErrorType.Forbidden,
                });
            }
            
            
            if (conversation.InitiatedByUserId == null ||
                conversation.DirectRequestStatus == null)
            {
                throw new InvalidOperationException(
                    "Direct conversation is missing its request information.");
            }

            if (conversation.DirectRequestStatus == DirectRequestStatus.Pending)
            {
                // The recipient must accept before sending.
                if (conversation.InitiatedByUserId != userId)
                {
                    return Result<SendMessageResponse>.Failure(new Error
                    {
                        Code = "Conversation.AcceptRequired",
                        Message = "Accept the message request before replying.",
                        Type = ErrorType.Forbidden,
                    });
                }

                // Only the initiator can reach this point while pending.
                int maxDirectPendingMessage = 3;

                var numberOfMessages = await _dbContext.Messages
                    .CountAsync(m =>
                        m.ConversationId == conversation.Id &&
                        m.SenderUserId == userId);

                if (numberOfMessages >= maxDirectPendingMessage)
                {
                    return Result<SendMessageResponse>.Failure(new Error
                    {
                        Code = "Conversation.LimitReached",
                        Message = "Wait for the recipient to accept before sending more messages.",
                        Type = ErrorType.BadRequest,
                    });
                }


            }
            else if (conversation.DirectRequestStatus != DirectRequestStatus.Accepted)
            {
                throw new InvalidOperationException(
                    "Unsupported direct conversation request status.");
            }
        }
        else
        {
            // Group messaging rules are not implemented yet.
            throw new NotImplementedException();
        }

          // edgecase check if 2 messages then count 2 then another count 2  (already exceeds limit)


          var participantIds = await _dbContext.ConversationMembers.Where(c => c.ConversationId == conversation.Id)
              .Select(c => c.UserId).ToListAsync();
          
        
        var message = new Message()
        {
            ClientMessageId = request.ClientMessageId,
            ConversationId = conversation.Id,
            SenderUserId = userId,
            Text = request.Text,
        };
        _dbContext.Messages.Add(message);
        await _dbContext.SaveChangesAsync();


        var response = new SendMessageResponse()
        {
            ConversationId = conversation.Id,
            MessageId = message.Id,
            SenderUserId = userId,
            CreatedAt = message.CreatedAt,
            Text = message.Text,
            SenderDisplayName = user.DisplayName ?? "unknown user",
            ClientMessageId = message.ClientMessageId,
        };
        
        try
        {
            await _chathub.Clients.Users(participantIds)
                .SendMessage(response);
        }
        catch (Exception exception)
        {
            _logger.LogWarning(
                exception,
                "Notification failed for saved message {MessageId}.",
                message.Id);
        }
        return Result<SendMessageResponse>.Success(response);
        

  


    }
}
