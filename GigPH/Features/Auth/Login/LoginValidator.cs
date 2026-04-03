using FluentValidation;

namespace GigPH.Features.Auth.Login;

public class LoginValidator : AbstractValidator<LoginRequest>
{
    public LoginValidator()
    {
        RuleFor(u => u.UsernameOrEmail)
            .NotEmpty()
            .EmailAddress();
        RuleFor(u => u.Password)
            .NotEmpty();
    }
    
}