using FluentValidation;

namespace PrjA.Features.User.Onboarding;

public class OnboardingValidator : AbstractValidator<OnboardingRequest>
{
    public OnboardingValidator()
    {
    }

}
