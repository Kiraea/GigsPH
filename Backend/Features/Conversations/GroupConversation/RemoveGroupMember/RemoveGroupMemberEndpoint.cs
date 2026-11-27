using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PrjA.Infrastructure;

namespace PrjA.Features.Conversations.GroupConversation.RemoveGroupMember;

[ApiController]
[Authorize]
[Route("/api/conversations/groups")]
public class RemoveGroupMemberEndpoint : ControllerBase
{
    private readonly RemoveGroupMemberHandler _handler;

    public RemoveGroupMemberEndpoint(RemoveGroupMemberHandler handler)
    {
        _handler = handler;
    }

    [HttpDelete("{conversationId:guid}/members/{memberUserId}")]
    public async Task<ActionResult<RemoveGroupMemberResponse>> RemoveGroupMember(
        [FromRoute] Guid conversationId, [FromRoute] string memberUserId)
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

        if (string.IsNullOrWhiteSpace(memberUserId))
        {
            ModelState.AddModelError(nameof(memberUserId), "Member user ID must not be empty.");
        }

        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var response = await _handler.HandleAsync(conversationId, memberUserId, userId);
        return response.isSuccess ? Ok(response.Value) : response.ToProblemDetails(this);
    }
}
