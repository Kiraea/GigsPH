namespace PrjA.Features.Conversations.SendMessage;

public class SendMessageRequest
{
    public required string Text { get; set; }
    public Guid ClientMessageId { get; set; }
}
