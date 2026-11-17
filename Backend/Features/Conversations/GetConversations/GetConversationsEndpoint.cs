using System.Security.Claims;
using FluentValidation;
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
    private readonly IValidator<GetConversationsRequest> _validator;

    public GetConversationsEndpoint(
        GetConversationsHandler handler, IValidator<GetConversationsRequest> validator)
    {
        _handler = handler;
        _validator = validator;
    }

    [HttpGet]
    public async Task<ActionResult<GetConversationsResponse>> GetConversations(
        [FromQuery] GetConversationsRequest request)
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
