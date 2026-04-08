using System.Security.Claims;
using FluentValidation;
using GigPH.Features.Post.CreatePost;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GigPH.Features.Post.UpdatePost;




[ApiController]
[Route("/api/posts")]
public class UpdatePostEndpoint: ControllerBase
{
    private readonly UpdatePostHandler _handler;
    private readonly IValidator<UpdatePostRequest> _validator;
    
    public UpdatePostEndpoint(UpdatePostHandler handler, IValidator<UpdatePostRequest> validator)
    {
        _handler = handler;
        _validator = validator;
    }

    [Authorize]
    [HttpPatch("{postId:guid}")]
    public async Task<ActionResult<UpdatePostResponse>> CreatePost(
        [FromForm] UpdatePostRequest request, [FromRoute] Guid postId)
    {

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userId, out var result))
        {
            return Unauthorized();
        }

        request.UserId = result;
        request.PostId = postId;
        
        var validationResponse = await _validator.ValidateAsync(request);
        if (!validationResponse.IsValid)
        {
            return ValidationProblem(validationResponse.ToString());
        }

        var response = await _handler.HandleAsync(request);
        return response;
    }

}

