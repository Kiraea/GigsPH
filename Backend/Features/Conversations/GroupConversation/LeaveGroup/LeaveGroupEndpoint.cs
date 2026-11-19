using System.Security.Claims;
using FluentValidation;
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
    private readonly IValidator<LeaveGroupRequest> _validator;

    public LeaveGroupEndpoint(
        LeaveGroupHandler handler, IValidator<LeaveGroupRequest> validator)
    {
        _handler = handler;
        _validator = validator;
    }

    [HttpPost("{conversationId:guid}/leave")]
    public async Task<ActionResult<LeaveGroupResponse>> LeaveGroup(
        [FromRoute] LeaveGroupRequest request)
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

