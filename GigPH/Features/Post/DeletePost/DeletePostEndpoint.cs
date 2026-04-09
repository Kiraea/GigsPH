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
         [FromRoute] Guid postId)
    {
        var result = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(result, out var requesterUserId))
        {
            return Unauthorized();
        }

        var response = await _handler.HandleAsync(requesterUserId, postId);
        return response;
    }

}

