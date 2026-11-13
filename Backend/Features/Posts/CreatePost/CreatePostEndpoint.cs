using System.Security.Claims;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PrjA.Infrastructure;

namespace PrjA.Features.Posts.CreatePost;

[ApiController]
[Authorize]
[Route("/api/posts")]
public class CreatePostEndpoint : ControllerBase
{
    private readonly CreatePostHandler _handler;
    private readonly IValidator<CreatePostRequest> _validator;

    public CreatePostEndpoint(CreatePostHandler handler, IValidator<CreatePostRequest> validator)
    {
        _handler = handler;
        _validator = validator;
    }

    [HttpPost]
    public async Task<ActionResult<CreatePostResponse>> CreatePost([FromBody] CreatePostRequest request)
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

        var response = await _handler.HandleAsync(request, userId);
        return response.isSuccess ? Ok(response.Value) : response.ToProblemDetails(this);
    }
}

