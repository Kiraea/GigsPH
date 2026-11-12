using FluentValidation;

namespace PrjA.Features.Auth.Register;

public class RegisterValidator : AbstractValidator<RegisterRequest>
{
    public RegisterValidator()
    {
        //RuleFor(x => x.Email).NotEmpty().EmailAddress();
        //RuleFor(x => x.Password).NotEmpty().MinimumLength(1).MaximumLength(2);
    }
}