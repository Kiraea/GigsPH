using FluentValidation;

namespace PrjA.Features.Conversations.GroupConversation.CreateGroupConversation;

public class CreateGroupConversationValidator : AbstractValidator<CreateGroupConversationRequest>
{
    public CreateGroupConversationValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.InvitedUserIds).NotEmpty();
        RuleForEach(x => x.InvitedUserIds).NotEmpty();
    }
}

