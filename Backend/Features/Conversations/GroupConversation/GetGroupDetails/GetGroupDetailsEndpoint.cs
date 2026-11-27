using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PrjA.Infrastructure;

namespace PrjA.Features.Conversations.GroupConversation.GetGroupDetails;

[ApiController]
[Authorize]
[Route("/api/conversations/groups")]
public class GetGroupDetailsEndpoint : ControllerBase
{
    private readonly GetGroupDetailsHandler _handler;

    public GetGroupDetailsEndpoint(GetGroupDetailsHandler handler)
    {
        _handler = handler;
    }

    [HttpGet("{conversationId:guid}")]
    public async Task<ActionResult<GetGroupDetailsResponse>> GetGroupDetails(
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
