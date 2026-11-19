using FluentValidation;

namespace PrjA.Features.Conversations.GroupConversation.LeaveGroup;

public class LeaveGroupValidator : AbstractValidator<LeaveGroupRequest>
{
    public LeaveGroupValidator()
    {
        RuleFor(x => x.ConversationId).NotEmpty();
    }
}

