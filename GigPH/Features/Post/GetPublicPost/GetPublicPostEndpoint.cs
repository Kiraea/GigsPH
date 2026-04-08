using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GigPH.Features.Post.GetPublicPost;

[ApiController]
[Route("/api/posts")]
public class GetPublicPostEndpoint: ControllerBase
{
    private readonly GetPublicPostHandler _handler;
    
    public GetPublicPostEndpoint(GetPublicPostHandler handler)
    {
        _handler = handler;
    }

    [HttpGet("{postId :guid}")]
    public async Task<ActionResult<GetPublicPostResponse>> GetPublicPost(GetPublicPostRequest request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userId, out var result))
        {
            return Unauthorized();
        }

        var response = await _handler.HandleAsync(request);
        return response;
    }

}

