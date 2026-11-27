using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PrjA.Infrastructure;

namespace PrjA.Features.Conversations.GetConversations;

[ApiController]
[Authorize]
[Route("/api/conversations")]
public class GetConversationsEndpoint : ControllerBase
{
    private readonly GetConversationsHandler _handler;

    public GetConversationsEndpoint(GetConversationsHandler handler)
    {
        _handler = handler;
    }

    [HttpGet]
    public async Task<ActionResult<GetConversationsResponse>> GetConversations()
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
