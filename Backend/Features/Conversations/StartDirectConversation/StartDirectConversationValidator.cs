using FluentValidation;

namespace PrjA.Features.Conversations.StartDirectConversation;

public class StartDirectConversationValidator : AbstractValidator<StartDirectConversationRequest>
{
    public StartDirectConversationValidator()
    {
        RuleFor(x => x.RecipientUserId).NotEmpty();
        RuleFor(x => x.Text).NotEmpty();
        RuleFor(x => x.ClientMessageId).NotEmpty();

        // TODO: Add a maximum text length once the message limit is decided.
    }
}
