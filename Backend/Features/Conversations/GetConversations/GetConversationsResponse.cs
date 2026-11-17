using PrjA.Domain;

namespace PrjA.Features.Conversations.GetConversations;


public class GetConversationsResponse
{
    public ICollection<GetConversationResponse> Conversations { get; set; } = [];
}
public class GetConversationResponse
{
    public Guid ConversationId { get; set; }
    public required string ConversationName { get; set; }
    public ConversationType ConversationType { get; set; }
    public DirectRequestStatus? DirectRequestStatus { get; set; }
    public bool CanAcceptRequest { get; set; } 
    public string? LatestMessage { get; set; }
    public string? LatestSender { get; set; } 
    public DateTime? LatestDate { get; set; } 
    
}
