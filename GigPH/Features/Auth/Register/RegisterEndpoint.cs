namespace GigPH.Features.Auth.Register;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("/api/auth")]
public class RegisterEndpoint : ControllerBase
{
    private readonly RegisterHandler _handler;
    private readonly IValidator<RegisterRequest> _validator;
    public RegisterEndpoint (RegisterHandler handler,IValidator<RegisterRequest> validator)
    {
        _handler = handler;
        _validator = validator;

    }
    [HttpPost("register")]
    public async Task<ActionResult<RegisterResponse>> Register([FromBody] RegisterRequest request)
    {
        var validation = await _validator.ValidateAsync(request);
        if (!validation.IsValid)
        {
            return ValidationProblem(validation.ToString());
        }

        var registerResponse = await _handler.HandleAsync(request);

        return registerResponse;


    }
}
