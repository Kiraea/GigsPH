namespace PrjA.Domain.Entities;

public class Message :ITimestamp
{
    public Guid Id { get; set; }
    public required string Text { get; set; }
    
    
    public Guid ConversationId  { get; set; }
    public required string SenderUserId { get; set; }
    public AppUser? SenderUser { get; set; }
    
    
    
    // this is basically if u send message "a" the backend gets it but the backend doesnt inform frontend what happen so frontend retries
    // that means u get two 'a'  if u resend so this prevents that cause if u just check if clientid already exist it sohuldnt send the other one
    public Guid ClientMessageId { get; set; }
    
    
    // we already have created at
    public DateTime UpdatedAt { get; set; }
    public DateTime CreatedAt { get; set; }
}