using FluentValidation;

namespace GigPH.Features.Auth.Login;

public class LoginValidator : AbstractValidator<LoginRequest>
{
    public LoginValidator()
    {
        
    }
    
}