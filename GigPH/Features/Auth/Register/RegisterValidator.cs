namespace GigPH.Features.Auth.Register;
using FluentValidation;



public class RegisterValidator : AbstractValidator<RegisterRequest>
{
    public RegisterValidator()
    {
        RuleFor(r => r.Email)
            .NotNull()
            .EmailAddress();

        RuleFor(r => r.Password)
            .NotNull()
            .MinimumLength(2);
    }
}