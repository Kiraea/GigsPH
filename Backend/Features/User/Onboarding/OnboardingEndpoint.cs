using System.Security.Claims;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PrjA.Features.User.Onboarding;



[ApiController]
[Route("/api/user/onboarding")]
public class OnboardingEndpoint: ControllerBase
{

    private readonly IValidator<OnboardingRequest> _validator;
    private readonly  OnboardingHandler _handler;
    public OnboardingEndpoint(IValidator<OnboardingRequest> validator, OnboardingHandler handler)
    {
        _validator = validator;
        _handler = handler;
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<OnboardingResponse>> Onboarding(OnboardingRequest request)
    {
        
        // The default JWT claim mapping translates "sub" into NameIdentifier.
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized();
        }

        var validateResult = await _validator.ValidateAsync(request);
        if (!validateResult.IsValid)
        {
            return InvalidRequest(validateResult.Errors);
        }

        try
        {
            var response = await _handler.HandleAsync(request, userId);
            return Ok(response);
        }
        catch (ValidationException exception)
        {
            return InvalidRequest(exception.Errors);
        }
        catch (KeyNotFoundException)
        {
            return NotFound(new ProblemDetails { Title = "User not found." });
        }
    }

    private ActionResult InvalidRequest(IEnumerable<ValidationFailure> errors)
    {
        foreach (var error in errors)
        {
            ModelState.AddModelError(error.PropertyName, error.ErrorMessage);
        }

        return ValidationProblem(ModelState);
    }
}
