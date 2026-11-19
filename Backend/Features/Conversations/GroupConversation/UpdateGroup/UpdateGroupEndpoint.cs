using System.Security.Claims;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PrjA.Infrastructure;

namespace PrjA.Features.Conversations.GroupConversation.UpdateGroup;

[ApiController]
[Authorize]
[Route("/api/conversations/groups")]
public class UpdateGroupEndpoint : ControllerBase
{
    private readonly UpdateGroupHandler _handler;
    private readonly IValidator<UpdateGroupRequest> _validator;

    public UpdateGroupEndpoint(
        UpdateGroupHandler handler, IValidator<UpdateGroupRequest> validator)
    {
        _handler = handler;
        _validator = validator;
    }

    [HttpPatch("{conversationId:guid}")]
    public async Task<ActionResult<UpdateGroupResponse>> UpdateGroup(
        [FromRoute] Guid conversationId, [FromBody] UpdateGroupRequest request)
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

