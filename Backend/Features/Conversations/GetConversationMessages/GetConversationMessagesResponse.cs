namespace PrjA.Features.Conversations.GetConversationMessages;

public class GetConversationMessagesResponse
{
    public Guid ConversationId { get; set; }

    public ICollection<MessageDto> Messages { get; set; } = [];
    public DateTimeOffset? Cursor { get; set; } 

}

public class MessageDto
{
    
    public Guid Id { get; set; }
    public required string Text { get; set; }
    
    public required string SenderUserId { get; set; }
    public required string SenderDisplayName { get; set; }
    
    
    
    public Guid ClientMessageId { get; set; }
    
    
    // we already have created at
    public DateTime CreatedAt { get; set; }
}
