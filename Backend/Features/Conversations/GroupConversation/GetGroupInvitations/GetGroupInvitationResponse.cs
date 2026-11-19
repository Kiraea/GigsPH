namespace PrjA.Features.Conversations.GroupConversation.GetGroupInvitations;

public class GetGroupInvitationResponse
{
    public Guid InvitationId { get; set; }
    public required string GroupName { get; set; }
}

