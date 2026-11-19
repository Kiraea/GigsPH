using PrjA.Domain.Entities;

namespace PrjA.Features.Conversations.GroupConversation.RespondToGroupInvitation;

public class RespondToGroupInvitationResponse
{
    public Guid InvitationId { get; set; }
    public Guid? ConversationId { get; set; }
    public GroupChatInvitationStatus Status { get; set; }
}

