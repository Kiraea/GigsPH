namespace PrjA.Features.Conversations.GroupConversation.RemoveGroupMember;

public class RemoveGroupMemberRequest
{
    public Guid ConversationId { get; set; }
    public required string MemberUserId { get; set; }
}

