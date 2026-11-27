using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PrjA.Common;
using PrjA.Domain;
using PrjA.Domain.Entities;
using PrjA.Infrastructure;

namespace PrjA.Features.Conversations.GetConversations;

public class GetConversationsHandler
{
    private readonly AppDbContext _dbContext;
    private readonly UserManager<AppUser> _userManager;

    public GetConversationsHandler(AppDbContext dbContext, UserManager<AppUser> userManager)
    {
        _dbContext = dbContext;
        _userManager = userManager;
    }

    public async Task<Result<GetConversationsResponse>> HandleAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return Result<GetConversationsResponse>.Failure(new Error
            {
                Code = "User.NotFound",
                Message = "User not found",
                Type = ErrorType.NotFound,
            });
        }

        // list of conversation of user
        var conversationMembers = await _dbContext.ConversationMembers.AsNoTracking().Where(cm => cm.UserId == userId).Include(cm => cm.Conversation)
            .ThenInclude(cm => cm.ConversationMembers).ThenInclude(cm2 => cm2.User).Include(cm => cm.Conversation)
            .ThenInclude(c => c.Messages
                .OrderByDescending(m => m.CreatedAt).ThenByDescending(m => m.Id)
                .Take(1)).ThenInclude(m => m.SenderUser).ToListAsync();
            
        
        
        // get conversation Type 

        
        
        // Check can acceptrequest 
        // to find out we check the convo initiator if its NOT him // and its direct type // and iTS pending -> if all true -> canacceptrequest
        // Find latest message an details

        var blockedUserIds = await _dbContext.UserBlocks
            .Where(b => b.BlockedUserId == userId || b.BlockerUserId == userId)
            .Select(b => b.BlockedUserId == userId ? b.BlockerUserId : b.BlockedUserId).ToListAsync();

        var conversations = conversationMembers.Select(cm =>
        {
            if (cm.Conversation == null)
            {
                throw new InvalidOperationException(
                    "Membership has no conversation.");
            }

 


            return new GetConversationResponse()
            {
                ConversationId = cm.Conversation.Id,
                ConversationType = cm.Conversation.Type,
                DirectRequestStatus = cm.Conversation.DirectRequestStatus,

                ConversationName = cm.Conversation.Type == ConversationType.Direct
                    ? cm.Conversation.ConversationMembers.Where(cm2 =>
                            cm2.ConversationId == cm.ConversationId && cm2.UserId != userId)
                        .Select(cm2 => cm2.User == null ? null : cm2.User.DisplayName).FirstOrDefault() ?? "UnknownUser"
                    : cm.Conversation.Name ?? "Group Chat",
                CanAcceptRequest = cm.Conversation.Type == ConversationType.Direct &&
                                   cm.Conversation.DirectRequestStatus == DirectRequestStatus.Pending &&
                                   cm.Conversation.InitiatedByUserId != userId && cm.Conversation.InitiatedByUserId != null && !cm.Conversation.ConversationMembers
                                       .Any(m => blockedUserIds.Contains(m.UserId)) ,
                
                LatestDate =  cm.Conversation.Messages.Select(m => (DateTime?)m.CreatedAt).FirstOrDefault(),
                LatestMessage=  cm.Conversation.Messages.Select(m => m.Text).FirstOrDefault(),
                LatestSender=  cm.Conversation.Messages.Select(m => m.SenderUser?.DisplayName ).FirstOrDefault(),

            };

        }).OrderByDescending(c => c.LatestDate).ThenBy(c => c.ConversationId).ToList();
      
        var response = new GetConversationsResponse
        {
            Conversations = conversations,
        };

        return Result<GetConversationsResponse>.Success(response);
        

    }
}
