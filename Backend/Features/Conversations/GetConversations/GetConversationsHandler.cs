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
        var conversations = await _dbContext.Conversations.AsNoTracking()
            .Where(c => c.ConversationMembers.Any(cm => cm.UserId == userId))
            .Include(c => c.ConversationMembers).ThenInclude(cm => cm.User)
            .Include(c => c.Messages
                .OrderByDescending(m => m.CreatedAt)
                .ThenByDescending(m => m.Id).Take(1))
            .ThenInclude(m => m.SenderUser)
            .ToListAsync();
        
        
            
        
        
        // get conversation Type 

        
        
        // Check can acceptrequest 
        // to find out we check the convo initiator if its NOT him // and its direct type // and iTS pending -> if all true -> canacceptrequest
        // Find latest message an details

        var blockedUserIds = await _dbContext.UserBlocks
            .Where(b => b.BlockedUserId == userId || b.BlockerUserId == userId)
            .Select(b => b.BlockedUserId == userId ? b.BlockerUserId : b.BlockedUserId).ToListAsync();



        var getConversationResponses= conversations.Select(c => new GetConversationResponse
        {
            ConversationName = c.Type == ConversationType.Direct
                ? c.ConversationMembers.FirstOrDefault(cm => cm.UserId != userId)?.User?.DisplayName ?? "Unknown User"
                : c.Name ?? "Unknown Group",
            ConversationType = c.Type,
            DirectRequestStatus = c.DirectRequestStatus,
            ConversationId = c.Id,

            CanAcceptRequest = c.InitiatedByUserId != userId && c.DirectRequestStatus == DirectRequestStatus.Pending && c.InitiatedByUserId != null && !c.ConversationMembers.Any(cm => blockedUserIds.Contains(cm.UserId)),
            LatestMessage = c.Messages.FirstOrDefault()?.Text,
            LatestDate = c.Messages.FirstOrDefault()?.CreatedAt,
            LatestSender = c.Messages.FirstOrDefault()?.SenderUser?.DisplayName,
        }).OrderByDescending(c => c.LatestDate).ThenByDescending(c => c.ConversationId).ToList();
        

       
        var response = new GetConversationsResponse
        {
            Conversations = getConversationResponses,
        };

        return Result<GetConversationsResponse>.Success(response);
        

    }
}
