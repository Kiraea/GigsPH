namespace PrjA.Features.Conversations.GetConversationMessages;

public class GetConversationMessagesRequest
{
    public int PageSize { get; set; } = 20;

    // TODO: Add pagination continuation fields once the pagination contract is decided.
}
