using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace PrjA.Features.Auth.Login;

[ApiController]
[Route("/api/auth/")]
public class LoginEndpoint : ControllerBase
{
    private readonly IValidator<LoginRequest> _validator;
    private readonly LoginHandler _handler;
    
    public LoginEndpoint(LoginHandler handler, IValidator<LoginRequest> validator)
    {
        _validator = validator;
        _handler = handler;
        
    }

    [HttpPost]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
    {
        var result = await _validator.ValidateAsync(request);
        if (!result.IsValid)
        {
            string errors = string.Join(Environment.NewLine, result.Errors.Select(e => e.ErrorMessage));
        }

        var response = await _handler.Handle(request);

        return Ok(response);

    }
    
    
}