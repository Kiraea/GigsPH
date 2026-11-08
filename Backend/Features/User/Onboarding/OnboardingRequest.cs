using PrjA.Domain;

namespace PrjA.Features.User.Onboarding;

public class OnboardingRequest
{
    public required string LastName { get; set; }
    public required string FirstName { get; set; }
    public required string DisplayName { get; set; }
    public string? Description { get; set; }
    public List<Guid> GenreIds { get; set; } = [];
    public List<OnboardingInstrumentRequest> Instruments { get; set; } = [];
    public List<OnboardingSocialLinkRequest> SocialLinks { get; set; } = [];
    public OnboardingLocationRequest Location { get; set; } 
}


public class OnboardingSocialLinkRequest
{
    public required Platform Platform { get; set; }
    public required string Url { get; set; } 
    
}

public class OnboardingLocationRequest
{
    public required string Country { get; set; } = string.Empty;
    public required string City { get; set; } = string.Empty;
    public required string ProvinceState { get; set; } = string.Empty;
    
}

public class OnboardingInstrumentRequest
{
    public Guid InstrumentId { get; set; }
    public bool IsMain { get; set; }
}
