using FluentValidation;

namespace PrjA.Features.Conversations.GroupConversation.UpdateGroup;

public class UpdateGroupValidator : AbstractValidator<UpdateGroupRequest>
{
    public UpdateGroupValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
    }
}

