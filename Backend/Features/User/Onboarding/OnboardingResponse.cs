namespace PrjA.Features.User.Onboarding;

public class OnboardingResponse
{
    public required string Id { get; set; }
    public string? DisplayName { get; set; }
    public bool IsOnboarded { get; set; }
}
