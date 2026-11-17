using FluentValidation;

namespace PrjA.Features.Conversations.RespondToDirectRequest;

public class RespondToDirectRequestValidator : AbstractValidator<RespondToDirectRequestRequest>
{
    public RespondToDirectRequestValidator()
    {
        RuleFor(x => x.Accept)
            .NotNull()
            .WithMessage("Specify true to accept or false to decline the request.");
    }
}
