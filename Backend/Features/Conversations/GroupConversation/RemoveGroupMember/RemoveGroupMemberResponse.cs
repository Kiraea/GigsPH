namespace PrjA.Features.Conversations.GroupConversation.RemoveGroupMember;

public class RemoveGroupMemberResponse
{
    public Guid ConversationId { get; set; }
    public required string RemovedUserId { get; set; }
}

