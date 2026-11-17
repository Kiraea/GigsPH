using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using PrjA.Common;
using PrjA.Domain;
using PrjA.Domain.Entities;
using PrjA.Features.Conversations.Realtime;
using PrjA.Features.Conversations.SendMessage;
using PrjA.Infrastructure;

namespace PrjA.Features.Conversations.StartDirectConversation;

public class StartDirectConversationHandler
{
    private readonly AppDbContext _dbContext;
    private readonly ILogger<StartDirectConversationHandler > _logger;
    private readonly IHubContext<Chathub, IChatClient> _chathub;
    
    private readonly UserManager<AppUser> _userManager;

    public StartDirectConversationHandler(AppDbContext dbContext, UserManager<AppUser> userManager, IHubContext<Chathub,  IChatClient> chathub, ILogger<StartDirectConversationHandler> logger)
    {
        _dbContext = dbContext;
        _userManager = userManager;
        _chathub = chathub;
        _logger = logger;
        
    }

    public async Task<Result<StartDirectConversationResponse>> HandleAsync(
        StartDirectConversationRequest request, string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return Result<StartDirectConversationResponse>.Failure(new Error
            {
                Code = "User.NotFound",
                Message = "User not found",
                Type = ErrorType.NotFound,
            });
        }

        var recipientId = await _userManager.FindByIdAsync(request.RecipientUserId);
        if (recipientId == null)
        {
            return Result<StartDirectConversationResponse>.Failure(new Error()
            {
                Code = "User.RecipientNotFound",
                Message = "Recipient not found",
                Type = ErrorType.NotFound,
            });
        }
        
        // check if theres existingConvo if so error
        
        var userIds = new[] { user.Id, request.RecipientUserId };


        
        // lets say convoId = 10
        // user 1,2,3 are in here
        
        // that means i only get 1,2
        
        // it only answers are these two people connected (it doesnt matter if there are people are also here such as usser3
        
        
        
        // if finding two var existConversation = _dbContext.ConversationMembers.GroupBy(c => c.ConversationId).Any(g => g.Any(g => userIds.Contains()) && g.Any(userIds))
        
        
        
        // nclude is for loading related objects into the returned entities_
        var existingConvo = await _dbContext.ConversationMembers.Where(cm => cm.Conversation.Type == ConversationType.Direct).GroupBy(g => g.ConversationId).AnyAsync(g =>  g.Select(m => m.UserId).Distinct().Count() == userIds.Length && g.All(g => userIds.Contains(g.UserId)));

        if (existingConvo)
        {
            return Result<StartDirectConversationResponse>.Failure(new Error()
            {
                Code = "User.ConvoAlreadyExists",
                Message = "Conversation already Exists",
                Type = ErrorType.BadRequest,
            });
        }
        
        var isBlocked = await _dbContext.UserBlocks.AnyAsync(u => 
        (u.BlockedUserId == request.RecipientUserId && u.BlockerUserId == userId) ||
        (u.BlockedUserId == userId && u.BlockerUserId == request.RecipientUserId));


        if (isBlocked)
        {
            return Result<StartDirectConversationResponse>.Failure(new Error()
            {
                Code = "User.IsBlocked",
                Message = "Blocked",
                Type = ErrorType.BadRequest,
            });
        }

        if (user.Id == recipientId.Id)
        {
            return Result<StartDirectConversationResponse>.Failure(new Error
            {
                Code = "Conversation.SelfContact",
                Message = "You cannot start a direct conversation with yourself.",
                Type = ErrorType.BadRequest,
            });
        }


        var conversation = new Conversation()
        {
            DirectRequestStatus = DirectRequestStatus.Pending,
            BandId = null,
            InitiatedByUserId = user.Id,
            // if direct chatjust null
            Name = null,
            Type = ConversationType.Direct,

        };
        _dbContext.Conversations.Add(conversation);
        var messageAlreadySent = await _dbContext.Messages.AnyAsync(m => m.ClientMessageId == request.ClientMessageId);
        if (messageAlreadySent)
        {
            
            return Result<StartDirectConversationResponse>.Failure(new Error()
            {
                Code = "User.AlreadySent",
                Message = "AlreadySent",
                Type = ErrorType.BadRequest,
            });
        }

        var message = new Message()
        {
            ClientMessageId = request.ClientMessageId,
            Text = request.Text,
            SenderUserId = user.Id,
            ConversationId =  conversation.Id,
        };
        _dbContext.Messages.Add(message);


        
        // Alice starts a chat with Bob, message ID = aaa.
        // Bob starts a chat with Alice, message ID = bbb._
        //edgecase fix later change unique
        _dbContext.ConversationMembers.AddRange(
            new ConversationMember
            {
                ConversationId = conversation.Id,
                UserId = user.Id,
            },
            new ConversationMember
            {
                ConversationId = conversation.Id,
                UserId = recipientId.Id,
            });



        await _dbContext.SaveChangesAsync();


        var notification = new SendMessageResponse()
        {
            ClientMessageId = request.ClientMessageId,
            ConversationId = conversation.Id,
            MessageId = message.Id,
            SenderDisplayName = user.DisplayName ?? "unknown user ",
            Text = request.Text,
            SenderUserId = user.Id,
            CreatedAt = message.CreatedAt,
        };
        

        try
        {
            var participantIds = new[] { user.Id, recipientId.Id };
            await _chathub.Clients.Users(participantIds)
                .SendMessage(notification);
        }
        catch (Exception exception)
        {
            _logger.LogWarning(
                exception,
                "Notification failed for saved message {MessageId}.",
                message.Id);
        }
        return Result<StartDirectConversationResponse>.Success(new StartDirectConversationResponse()
        {
            ConversationId = conversation.Id,
        });
        
    }
}
