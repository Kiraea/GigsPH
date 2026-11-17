using System.Security.Claims;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using PrjA.Features.Conversations.Realtime;
using PrjA.Infrastructure;

namespace PrjA.Features.Conversations.RespondToDirectRequest;

[ApiController]
[Authorize]
[Route("/api/conversations")]
public class RespondToDirectRequestEndpoint : ControllerBase
{
    private readonly RespondToDirectRequestHandler _handler;
    private readonly IValidator<RespondToDirectRequestRequest> _validator;

    public RespondToDirectRequestEndpoint(
        RespondToDirectRequestHandler handler, IValidator<RespondToDirectRequestRequest> validator,)
    {
        _handler = handler;
        _validator = validator;
    }

    [HttpPost("{conversationId:guid}/request-response")]
    public async Task<ActionResult<RespondToDirectRequestResponse>> RespondToDirectRequest(
        [FromRoute] Guid conversationId, [FromBody] RespondToDirectRequestRequest request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized();
        }

        if (conversationId == Guid.Empty)
        {
            ModelState.AddModelError(nameof(conversationId), "Conversation ID must not be empty.");
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

        var response = await _handler.HandleAsync(conversationId, request, userId);
        return response.isSuccess ? Ok(response.Value) : response.ToProblemDetails(this);
    }
}
