using GigPH.Features.User.Shared.Dto;

namespace GigPH.Features.User.Onboard;

public record OnboardRequest
{
    public Guid UserId { get; set; }
    public string? DisplayName { get; init; }
    public string? FirstName{ get; init; }
    public string? LastName{ get; init; }
    public string? Description { get; init; }

    public List<Guid> GenreIds { get; init; } = [];
    public List<Guid> InstrumentIds { get; init; } = [];

    public List<SocialLinkRequest> SocialLinks { get; init; } = [];
    
    public string? Country { get; init; }
    public string? City { get; init; }
    public string? ProvinceState { get; init; }




    // lets just make everything required so fix in validators
}