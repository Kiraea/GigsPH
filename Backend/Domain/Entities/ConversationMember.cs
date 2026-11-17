namespace PrjA.Domain.Entities;

public class ConversationMember :ITimestamp
{
    public Guid ConversationId { get; set; }
    public Conversation? Conversation { get; set; }
    
    
    public required string UserId { get; set; }
    public AppUser? User { get; set; }
    
    
    
    public DateTime UpdatedAt { get; set; }
    public DateTime CreatedAt { get; set; }
}