using FluentValidation;

namespace PrjA.Features.Conversations.GroupConversation.GetGroupDetails;

public class GetGroupDetailsValidator : AbstractValidator<GetGroupDetailsRequest>
{
    public GetGroupDetailsValidator()
    {
        RuleFor(x => x.ConversationId).NotEmpty();
    }
}

