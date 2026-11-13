using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using PrjA.Infrastructure;

namespace PrjA.Features.Posts.GetPost;

[ApiController]
[Route("/api/posts")]
public class GetPostEndpoint : ControllerBase
{
    private readonly GetPostHandler _handler;
    private readonly IValidator<GetPostRequest> _validator;

    public GetPostEndpoint(GetPostHandler handler, IValidator<GetPostRequest> validator)
    {
        _handler = handler;
        _validator = validator;
    }

    [HttpGet("{postId:guid}")]
    public async Task<ActionResult<GetPostResponse>> GetPost([FromRoute] GetPostRequest request)
    {
        var result = await _validator.ValidateAsync(request);
        if (!result.IsValid)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
            }

            return ValidationProblem(ModelState);
        }

        var response = await _handler.HandleAsync(request);
        return response.isSuccess ? Ok(response.Value) : response.ToProblemDetails(this);
    }
}

