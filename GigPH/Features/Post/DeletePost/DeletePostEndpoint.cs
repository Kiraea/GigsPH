using System.Security.Claims;
using FluentValidation;
using GigPH.Features.Post.CreatePost;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GigPH.Features.Post.DeletePost;



[ApiController]
[Route("/api/posts")]
public class DeletePostEndpoint: ControllerBase
{
    private readonly DeletePostHandler _handler;
    
    public DeletePostEndpoint(DeletePostHandler handler)
    {
        _handler = handler;
    }

    [Authorize]
    [HttpDelete("{postId:guid}")]
    public async Task<ActionResult<DeletePostResponse>> DeletePost(
         [FromRoute] DeletePostRequest request, [FromRoute] Guid postId)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userId, out var result))
        {
            return Unauthorized();
        }

        request.UserId = result;
        request.PostId= postId;
        

        var response = await _handler.HandleAsync(request);
        return response;
    }

}

