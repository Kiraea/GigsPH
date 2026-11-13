using System.Security.Claims;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PrjA.Infrastructure;

namespace PrjA.Features.Posts.SetLikeStatusPost;

[ApiController]
[Authorize]
[Route("/api/posts")]
public class SetLikeStatusPostEndpoint : ControllerBase
{
    private readonly SetLikeStatusPostHandler _handler;
    private readonly IValidator<SetLikeStatusPostRequest> _validator;

    public SetLikeStatusPostEndpoint(SetLikeStatusPostHandler handler, IValidator<SetLikeStatusPostRequest> validator)
    {
        _handler = handler;
        _validator = validator;
    }

    [HttpPut("{postId:guid}/like")]
    public async Task<ActionResult<SetLikeStatusPostResponse>> SetLikeStatusPost([FromRoute] Guid postId, [FromBody] SetLikeStatusPostRequest request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized();
        }

        var result = await _validator.ValidateAsync(request);
        if (!result.IsValid)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
            }

            return ValidationProblem(ModelState);
        }

        var response = await _handler.HandleAsync(postId, request, userId);
        return response.isSuccess ? Ok(response.Value) : response.ToProblemDetails(this);
    }
}

