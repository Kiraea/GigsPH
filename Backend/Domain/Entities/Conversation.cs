namespace PrjA.Domain.Entities;

public class Conversation : ITimestamp
{
    public Guid Id { get; set; }
    
    public ConversationType Type { get; set; }
    public string? Name { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    
    public string? InitiatedByUserId { get; set; }
    public DirectRequestStatus? DirectRequestStatus { get; set; }

    public ICollection<Message> Messages { get; set; } = [];
    
    public ICollection<ConversationMember> ConversationMembers { get; set; } = [];



}
