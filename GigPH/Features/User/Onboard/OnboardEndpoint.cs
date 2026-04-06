using System.Security.Claims;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GigPH.Features.User.Onboard;



[ApiController]
[Route("/api/users")]
public class OnboardEndpoint : ControllerBase
{
    private readonly OnboardHandler _handler;
    private readonly IValidator<OnboardRequest> _validator;
    public OnboardEndpoint(OnboardHandler handler, IValidator<OnboardRequest> validator)
    {
        _handler = handler;
        _validator = validator;
    }


    [Authorize]
    [HttpPut("onboard")]
    public async Task<ActionResult<OnboardResponse>> Onboard([FromBody] OnboardRequest request)
    {
        
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(userId, out var result))
        {
            return Unauthorized();
        }

        request.UserId = result;

        var validation = await _validator.ValidateAsync(request);
        if (!validation.IsValid)
        {
            return ValidationProblem(validation.ToString());
        }

        var response = await _handler.HandleAsync(request);

        return response;

    }
}