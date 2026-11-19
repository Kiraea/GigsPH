namespace PrjA.Domain.Entities;

public class GroupChatInvitation
{
    public Guid id { get; set; }
    
    public Guid ConversationId { get; set; }
    public Conversation? Conversation { get; set; }
    public required string InvitedByUserId { get; set; }
    public required string InvitedToUserId { get; set; }
    public GroupChatInvitationStatus Status { get; set; }
    
    
}


public enum GroupChatInvitationStatus
{
    Accepted,
    Pending,
    Declined,
}