using System.Security.Claims;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PrjA.Infrastructure;

namespace PrjA.Features.Posts.UpdatePost;

[ApiController]
[Authorize]
[Route("/api/posts")]
public class UpdatePostEndpoint : ControllerBase
{
    private readonly UpdatePostHandler _handler;
    private readonly IValidator<UpdatePostRequest> _validator;

    public UpdatePostEndpoint(UpdatePostHandler handler, IValidator<UpdatePostRequest> validator)
    {
        _handler = handler;
        _validator = validator;
    }

    [HttpPatch("{postId:guid}")]
    public async Task<ActionResult<UpdatePostResponse>> UpdatePost([FromRoute] Guid postId, [FromBody] UpdatePostRequest request)
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

