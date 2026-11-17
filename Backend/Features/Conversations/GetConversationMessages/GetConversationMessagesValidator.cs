using FluentValidation;

namespace PrjA.Features.Conversations.GetConversationMessages;

public class GetConversationMessagesValidator : AbstractValidator<GetConversationMessagesRequest>
{
    public GetConversationMessagesValidator()
    {
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
    }
}
