using System.Security.Claims;
using FluentValidation;
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
    private readonly IValidator<GetGroupInvitationsRequest> _validator;

    public GetGroupInvitationsEndpoint(
        GetGroupInvitationsHandler handler, IValidator<GetGroupInvitationsRequest> validator)
    {
        _handler = handler;
        _validator = validator;
    }

    [HttpGet("invitations")]
    public async Task<ActionResult<List<GetGroupInvitationResponse>>> GetGroupInvitations(
        [FromQuery] GetGroupInvitationsRequest request)
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

