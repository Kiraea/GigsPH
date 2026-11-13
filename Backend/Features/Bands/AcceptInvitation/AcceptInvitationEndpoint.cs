using System.Security.Claims;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PrjA.Infrastructure;

namespace PrjA.Features.Bands.AcceptInvitation;

[ApiController]
[Authorize]
[Route("/api/bands")]
public class AcceptInvitationEndpoint : ControllerBase
{
    private readonly AcceptInvitationHandler _handler;
    private readonly IValidator<AcceptInvitationRequest> _validator;

    public AcceptInvitationEndpoint(
        AcceptInvitationHandler handler,
        IValidator<AcceptInvitationRequest> validator)
    {
        _handler = handler;
        _validator = validator;
    }

    [HttpPost("invitations/accept")]
    public async Task<ActionResult<AcceptInvitationResponse>> AcceptInvitation(
        [FromBody] AcceptInvitationRequest request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized();
        }

        var result = await _validator.ValidateAsync(request);
        if (!result.IsValid)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
            }
            return ValidationProblem(ModelState);
        }

        var response = await _handler.HandleAsync(request, userId);
        return response.isSuccess ? Ok(response.Value) : response.ToProblemDetails(this);
    }
}
