using System.Security.Claims;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PrjA.Infrastructure;

namespace PrjA.Features.Conversations.GetConversationMessages;

[ApiController]
[Authorize]
[Route("/api/conversations")]
public class GetConversationMessagesEndpoint : ControllerBase
{
    private readonly GetConversationMessagesHandler _handler;

    public GetConversationMessagesEndpoint(
        GetConversationMessagesHandler handler )
    {
        _handler = handler;
    }

    [HttpGet("{conversationId:guid}/messages")]
    public async Task<ActionResult<GetConversationMessagesResponse>> GetConversationMessages(
        [FromRoute] Guid conversationId, [FromQuery] DateTimeOffset? cursor)
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

        var response = await _handler.HandleAsync(conversationId ,userId, cursor);
        return response.isSuccess ? Ok(response.Value) : response.ToProblemDetails(this);
    }
}
