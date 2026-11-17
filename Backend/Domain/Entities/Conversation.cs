namespace PrjA.Domain.Entities;

public class Conversation : ITimestamp
{
    public Guid Id { get; set; }
    
    public ConversationType Type { get; set; }
    public string? Name { get; set; }
    public Guid? BandId { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime CreatedAt { get; set; }
    
    public string? InitiatedByUserId { get; set; }
    public DirectRequestStatus? DirectRequestStatus { get; set; }

    public ICollection<Message> Messages { get; set; } = [];
    
    // this is the limit if the user hasnt accepted u u can only send 3 messages to him max
    public ICollection<ConversationMember> ConversationMembers { get; set; } = [];



}