using System.Security.Claims;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PrjA.Infrastructure;

namespace PrjA.Features.Conversations.GroupConversation.TransferGroupOwnership;

[ApiController]
[Authorize]
[Route("/api/conversations/groups")]
public class TransferGroupOwnershipEndpoint : ControllerBase
{
    private readonly TransferGroupOwnershipHandler _handler;
    private readonly IValidator<TransferGroupOwnershipRequest> _validator;

    public TransferGroupOwnershipEndpoint(
        TransferGroupOwnershipHandler handler, IValidator<TransferGroupOwnershipRequest> validator)
    {
        _handler = handler;
        _validator = validator;
    }

    [HttpPut("{conversationId:guid}/owner")]
    public async Task<ActionResult<TransferGroupOwnershipResponse>> TransferGroupOwnership(
        [FromRoute] Guid conversationId, [FromBody] TransferGroupOwnershipRequest request)
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

