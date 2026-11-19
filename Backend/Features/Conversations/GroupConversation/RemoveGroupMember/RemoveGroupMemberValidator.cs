using FluentValidation;

namespace PrjA.Features.Conversations.GroupConversation.RemoveGroupMember;

public class RemoveGroupMemberValidator : AbstractValidator<RemoveGroupMemberRequest>
{
    public RemoveGroupMemberValidator()
    {
        RuleFor(x => x.ConversationId).NotEmpty();
        RuleFor(x => x.MemberUserId).NotEmpty();
    }
}

