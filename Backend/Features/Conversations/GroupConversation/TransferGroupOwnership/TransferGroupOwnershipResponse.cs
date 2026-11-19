namespace PrjA.Features.Conversations.GroupConversation.TransferGroupOwnership;

public class TransferGroupOwnershipResponse
{
    public Guid ConversationId { get; set; }
    public required string OwnerUserId { get; set; }
}

