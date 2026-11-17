namespace PrjA.Features.Conversations.SendMessage;

public class SendMessageResponse
{
    
    public Guid ConversationId { get; set; }
    public Guid MessageId { get; set; }
    
    public Guid ClientMessageId { get; set; }
    public required string SenderUserId { get; set; }
    
    public required string Text { get; set; }
    public required string SenderDisplayName { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    
    
    
    
    
}
