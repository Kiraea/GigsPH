using System.Security.Claims;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PrjA.Infrastructure;

namespace PrjA.Features.Conversations.GroupConversation.InviteGroupMember;

[ApiController]
[Authorize]
[Route("/api/conversations/groups")]
public class InviteGroupMemberEndpoint : ControllerBase
{
    private readonly InviteGroupMemberHandler _handler;
    private readonly IValidator<InviteGroupMemberRequest> _validator;

    public InviteGroupMemberEndpoint(
        InviteGroupMemberHandler handler, IValidator<InviteGroupMemberRequest> validator)
    {
        _handler = handler;
        _validator = validator;
    }

    [HttpPost("{conversationId:guid}/invitations")]
    public async Task<ActionResult<InviteGroupMemberResponse>> InviteGroupMember(
        [FromRoute] Guid conversationId, [FromBody] InviteGroupMemberRequest request)
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

