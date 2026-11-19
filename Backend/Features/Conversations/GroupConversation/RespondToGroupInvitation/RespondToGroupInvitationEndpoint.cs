using System.Security.Claims;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PrjA.Infrastructure;

namespace PrjA.Features.Conversations.GroupConversation.RespondToGroupInvitation;

[ApiController]
[Authorize]
[Route("/api/conversations/groups")]
public class RespondToGroupInvitationEndpoint : ControllerBase
{
    private readonly RespondToGroupInvitationHandler _handler;
    private readonly IValidator<RespondToGroupInvitationRequest> _validator;

    public RespondToGroupInvitationEndpoint(
        RespondToGroupInvitationHandler handler, IValidator<RespondToGroupInvitationRequest> validator)
    {
        _handler = handler;
        _validator = validator;
    }

    [HttpPost("invitations/{invitationId:guid}/response")]
    public async Task<ActionResult<RespondToGroupInvitationResponse>> RespondToGroupInvitation(
        [FromRoute] Guid invitationId, [FromBody] RespondToGroupInvitationRequest request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized();
        }

        if (invitationId == Guid.Empty)
        {
            ModelState.AddModelError(nameof(invitationId), "Invitation ID must not be empty.");
        }

        var result = await _validator.ValidateAsync(request);
        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
        }

        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var response = await _handler.HandleAsync(invitationId, request, userId);
        return response.isSuccess ? Ok(response.Value) : response.ToProblemDetails(this);
    }
}

