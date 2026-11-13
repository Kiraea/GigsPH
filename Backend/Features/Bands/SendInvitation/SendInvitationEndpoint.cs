using System.Security.Claims;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PrjA.Infrastructure;

namespace PrjA.Features.Bands.SendInvitation;

[ApiController]
[Authorize]
[Route("/api/bands")]
public class SendInvitationEndpoint: ControllerBase
{
    private readonly SendInvitationHandler _handler;
    private readonly IValidator<SendInvitationRequest> _validator;

    public SendInvitationEndpoint(SendInvitationHandler handler, IValidator<SendInvitationRequest> validator)
    {
        _handler = handler;
        _validator = validator;
    }

    [HttpPost("invitations")]
    public async Task<ActionResult<SendInvitationResponse>> SendInvitation([FromBody] SendInvitationRequest request)
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
