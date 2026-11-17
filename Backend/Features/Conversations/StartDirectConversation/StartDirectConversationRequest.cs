namespace PrjA.Features.Conversations.StartDirectConversation;

public class StartDirectConversationRequest
{
    public required string RecipientUserId { get; set; }
    public required string Text { get; set; }
    public Guid ClientMessageId { get; set; }
}
