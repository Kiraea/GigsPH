using System.Security.Claims;
using FluentValidation;
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
    private readonly IValidator<RemoveGroupMemberRequest> _validator;

    public RemoveGroupMemberEndpoint(
        RemoveGroupMemberHandler handler, IValidator<RemoveGroupMemberRequest> validator)
    {
        _handler = handler;
        _validator = validator;
    }

    [HttpDelete("{conversationId:guid}/members/{memberUserId}")]
    public async Task<ActionResult<RemoveGroupMemberResponse>> RemoveGroupMember(
        [FromRoute] RemoveGroupMemberRequest request)
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

