using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PrjA.Infrastructure;

namespace PrjA.Features.Conversations.GroupConversation.LeaveGroup;

[ApiController]
[Authorize]
[Route("/api/conversations/groups")]
public class LeaveGroupEndpoint : ControllerBase
{
    private readonly LeaveGroupHandler _handler;

    public LeaveGroupEndpoint(LeaveGroupHandler handler)
    {
        _handler = handler;
    }

    [HttpPost("{conversationId:guid}/leave")]
    public async Task<ActionResult<LeaveGroupResponse>> LeaveGroup(
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
