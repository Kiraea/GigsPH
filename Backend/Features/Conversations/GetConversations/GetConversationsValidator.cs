using FluentValidation;

namespace PrjA.Features.Conversations.GetConversations;

public class GetConversationsValidator : AbstractValidator<GetConversationsRequest>
{
    public GetConversationsValidator()
    {
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
    }
}
