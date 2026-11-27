using System.Security.Claims;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PrjA.Infrastructure;

namespace PrjA.Features.Bands.SendInvitation;

[ApiController]
[Authorize]
[Route("/api/bands")]
public class SendInvitationEndpoint : ControllerBase
{
    private readonly SendInvitationHandler _handler;
    private readonly IValidator<SendInvitationRequest> _validator;

    public SendInvitationEndpoint(SendInvitationHandler handler, IValidator<SendInvitationRequest> validator)
    {
        _handler = handler;
        _validator = validator;
    }

    [HttpPost("{bandId:guid}/invitations")]
    public async Task<ActionResult<SendInvitationResponse>> SendInvitation(
        [FromRoute] Guid bandId,
        [FromBody] SendInvitationRequest request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized();
        }

        if (bandId == Guid.Empty)
        {
            ModelState.AddModelError(nameof(bandId), "Band ID must not be empty.");
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

        var response = await _handler.HandleAsync(bandId, request, userId);
        return response.isSuccess ? Ok(response.Value) : response.ToProblemDetails(this);
    }
}
