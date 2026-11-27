using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PrjA.Infrastructure;

namespace PrjA.Features.Conversations.GroupConversation.GetGroupInvitations;

[ApiController]
[Authorize]
[Route("/api/conversations/groups")]
public class GetGroupInvitationsEndpoint : ControllerBase
{
    private readonly GetGroupInvitationsHandler _handler;

    public GetGroupInvitationsEndpoint(GetGroupInvitationsHandler handler)
    {
        _handler = handler;
    }

    [HttpGet("invitations")]
    public async Task<ActionResult<List<GetGroupInvitationResponse>>> GetGroupInvitations()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized();
        }

        var response = await _handler.HandleAsync(userId);
        return response.isSuccess ? Ok(response.Value) : response.ToProblemDetails(this);
    }
}
