using FluentValidation;

namespace PrjA.Features.User.Onboarding;

public class OnboardingValidator : AbstractValidator<OnboardingRequest>
{
    public OnboardingValidator()
    {
        RuleFor(request => request.DisplayName).NotEmpty();

        RuleFor(request => request.GenreIds)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .Must(ids => ids.Distinct().Count() == ids.Count)
            .WithMessage("Select each genre only once.");
        RuleForEach(request => request.GenreIds).NotEmpty();

        RuleFor(request => request.Instruments)
            .Cascade(CascadeMode.Stop)
            .NotNull()
            .Must(selections => selections.Select(selection => selection?.InstrumentId)
                .Distinct().Count() == selections.Count)
            .WithMessage("Select each instrument only once.");
        RuleForEach(request => request.Instruments).NotNull().ChildRules(instrument =>
        {
            instrument.RuleFor(selection => selection.InstrumentId).NotEmpty();
        });

        RuleFor(request => request.SocialLinks).NotNull();
        RuleForEach(request => request.SocialLinks).NotNull().ChildRules(link =>
        {
            link.RuleFor(value => value.Platform).IsInEnum();
            link.RuleFor(value => value.Url)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .Must(url => Uri.TryCreate(url, UriKind.Absolute, out var uri) &&
                    (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps))
                .WithMessage("Enter an absolute HTTP or HTTPS URL.");
        });

        RuleFor(request => request.Location).NotNull().ChildRules(location =>
        {
            location.RuleFor(value => value.Country).NotEmpty();
            location.RuleFor(value => value.ProvinceState).NotEmpty();
            location.RuleFor(value => value.City).NotEmpty();
        });
    }
    
}
