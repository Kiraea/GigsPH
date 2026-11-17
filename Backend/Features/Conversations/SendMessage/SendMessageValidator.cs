using FluentValidation;

namespace PrjA.Features.Conversations.SendMessage;

public class SendMessageValidator : AbstractValidator<SendMessageRequest>
{
    public SendMessageValidator()
    {
        RuleFor(x => x.Text).NotEmpty();
        RuleFor(x => x.ClientMessageId).NotEmpty();

        // TODO: Add a maximum text length once the message limit is decided.
    }
}
