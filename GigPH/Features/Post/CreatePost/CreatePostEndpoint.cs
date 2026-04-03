using System.Security.Claims;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GigPH.Features.Post.CreatePost;




[ApiController]
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

    [Authorize]
    [HttpPost]
    public async Task<ActionResult<CreatePostResponse>> CreatePost(
        [FromForm] CreatePostRequest request)
    {

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userId, out var result))
        {
            return Unauthorized();
        }

        request.UserId = result;
        
        var validationResponse = await _validator.ValidateAsync(request);
        if (!validationResponse.IsValid)
        {
            return ValidationProblem(validationResponse.ToString());
        }

        var response = await _handler.HandleAsync(request);
        return response;
    }

}

