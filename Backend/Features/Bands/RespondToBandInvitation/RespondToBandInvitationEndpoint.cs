using System.Security.Claims;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PrjA.Infrastructure;

namespace PrjA.Features.Bands.RespondToBandInvitation;

[ApiController]
[Authorize]
[Route("/api/bands")]
public class RespondToBandInvitationEndpoint : ControllerBase
{
    private readonly RespondToBandInvitationHandler _handler;
    private readonly IValidator<RespondToBandInvitationRequest> _validator;

    public RespondToBandInvitationEndpoint(
        RespondToBandInvitationHandler handler,
        IValidator<RespondToBandInvitationRequest> validator)
    {
        _handler = handler;
        _validator = validator;
    }

    [HttpPost("invitations/{invitationId:guid}/response")]
    public async Task<ActionResult<RespondToBandInvitationResponse>> RespondToBandInvitation(
        [FromRoute] Guid invitationId,
        [FromBody] RespondToBandInvitationRequest request)
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
