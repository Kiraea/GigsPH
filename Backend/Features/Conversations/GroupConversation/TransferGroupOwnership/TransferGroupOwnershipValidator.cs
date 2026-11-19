using FluentValidation;

namespace PrjA.Features.Conversations.GroupConversation.TransferGroupOwnership;

public class TransferGroupOwnershipValidator : AbstractValidator<TransferGroupOwnershipRequest>
{
    public TransferGroupOwnershipValidator()
    {
        RuleFor(x => x.NewOwnerUserId).NotEmpty();
    }
}

