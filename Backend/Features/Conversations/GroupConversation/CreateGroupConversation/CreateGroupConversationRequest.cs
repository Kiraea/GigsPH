namespace PrjA.Features.Conversations.GroupConversation.CreateGroupConversation;

public class CreateGroupConversationRequest
{
    public required string Name { get; set; }
    public ICollection<string> InvitedUserIds { get; set; } = [];
}

