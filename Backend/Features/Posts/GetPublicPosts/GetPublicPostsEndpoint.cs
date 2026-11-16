using System.Security.Claims;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PrjA.Infrastructure;

namespace PrjA.Features.Posts.GetPublicPosts;

[ApiController]
[Route("/api/posts")]
public class GetPublicPostsEndpoint : ControllerBase
{
    private readonly GetPublicPostsHandler _handler;
    private readonly IValidator<GetPublicPostsRequest> _validator;

    public GetPublicPostsEndpoint(GetPublicPostsHandler handler, IValidator<GetPublicPostsRequest> validator)
    {
        _handler = handler;
        _validator = validator;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<GetPublicPostsResponse>> GetPublicPosts([FromQuery] GetPublicPostsRequest request)
    {
        
        string? userId = null;
        if (HttpContext.User.Identity?.IsAuthenticated == true)
        {
            userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
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

