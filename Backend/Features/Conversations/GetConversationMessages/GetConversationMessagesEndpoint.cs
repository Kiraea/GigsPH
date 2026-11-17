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
    private readonly IValidator<GetConversationMessagesRequest> _validator;

    public GetConversationMessagesEndpoint(
        GetConversationMessagesHandler handler, IValidator<GetConversationMessagesRequest> validator)
    {
        _handler = handler;
        _validator = validator;
    }

    [HttpGet("{conversationId:guid}/messages")]
    public async Task<ActionResult<GetConversationMessagesResponse>> GetConversationMessages(
        [FromRoute] Guid conversationId, [FromQuery] GetConversationMessagesRequest request)
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
