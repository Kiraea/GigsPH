namespace PrjA.Features.Conversations.GroupConversation.GetGroupDetails;

public class GetGroupDetailsResponse
{
    
    public Guid ConversationId { get; set; }
    public required string Name { get; set; }

    public ICollection<UserGroupChatDetail> UserGroupChatDetails { get; set; } = [];
    public bool IsOwner { get; set; }
}



public class UserGroupChatDetail
{
    public required string UserId { get; set; }
    public required string DisplayName { get; set; }
    
}

