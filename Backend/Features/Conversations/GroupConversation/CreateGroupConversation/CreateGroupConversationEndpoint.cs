using System.Security.Claims;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PrjA.Infrastructure;

namespace PrjA.Features.Conversations.GroupConversation.CreateGroupConversation;

[ApiController]
[Authorize]
[Route("/api/conversations/groups")]
public class CreateGroupConversationEndpoint : ControllerBase
{
    private readonly CreateGroupConversationHandler _handler;
    private readonly IValidator<CreateGroupConversationRequest> _validator;

    public CreateGroupConversationEndpoint(
        CreateGroupConversationHandler handler, IValidator<CreateGroupConversationRequest> validator)
    {
        _handler = handler;
        _validator = validator;
    }

    [HttpPost]
    public async Task<ActionResult<CreateGroupConversationResponse>> CreateGroupConversation(
        [FromBody] CreateGroupConversationRequest request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized();
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

        var response = await _handler.HandleAsync(request, userId);
        return response.isSuccess ? Ok(response.Value) : response.ToProblemDetails(this);
    }
}

