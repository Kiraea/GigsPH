using System.Security.Claims;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PrjA.Infrastructure;

namespace PrjA.Features.Posts.DeletePost;

[ApiController]
[Authorize]
[Route("/api/posts")]
public class DeletePostEndpoint : ControllerBase
{
    private readonly DeletePostHandler _handler;
    private readonly IValidator<DeletePostRequest> _validator;

    public DeletePostEndpoint(DeletePostHandler handler, IValidator<DeletePostRequest> validator)
    {
        _handler = handler;
        _validator = validator;
    }

    [HttpDelete("{postId:guid}")]
    public async Task<ActionResult<DeletePostResponse>> DeletePost([FromRoute] DeletePostRequest request)
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

