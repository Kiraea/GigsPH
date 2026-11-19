using FluentValidation;

namespace PrjA.Features.Conversations.GroupConversation.InviteGroupMember;

public class InviteGroupMemberValidator : AbstractValidator<InviteGroupMemberRequest>
{
    public InviteGroupMemberValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
    }
}

