using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PrjA.Infrastructure;

namespace PrjA.Features.Conversations.RespondToDirectRequest;

[ApiController]
[Authorize]
[Route("/api/conversations")]
public class RespondToDirectRequestEndpoint : ControllerBase
{
    private readonly RespondToDirectRequestHandler _handler;

    public RespondToDirectRequestEndpoint(RespondToDirectRequestHandler handler)
    {
        _handler = handler;
    }

    [HttpPost("{conversationId:guid}/accept")]
    public async Task<ActionResult<RespondToDirectRequestResponse>> AcceptDirectRequest(
        [FromRoute] Guid conversationId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized();
        }

        if (conversationId == Guid.Empty)
        {
            ModelState.AddModelError(nameof(conversationId), "Conversation ID must not be empty.");
            return ValidationProblem(ModelState);
        }

        var response = await _handler.HandleAsync(conversationId, userId);
        return response.isSuccess ? Ok(response.Value) : response.ToProblemDetails(this);
    }
}
