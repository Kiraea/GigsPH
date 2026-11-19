using FluentValidation;

namespace PrjA.Features.Conversations.GroupConversation.RespondToGroupInvitation;

public class RespondToGroupInvitationValidator : AbstractValidator<RespondToGroupInvitationRequest>
{
    public RespondToGroupInvitationValidator()
    {
        // Accept=false is a valid rejection; do not validate it with NotEmpty().
    }
}

