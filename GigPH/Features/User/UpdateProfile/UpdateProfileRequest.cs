namespace GigPH.Features.User.UpdateProfile;

public record UpdateProfileRequest()
{
    public string? Description { get; init; }
    public List<Guid> InstrumentIds { get; init; } = [];
    public List<Guid> GenreIds { get; init; } = [];
    public List<string> SocialLinks { get; init; } = [];
    
    
    public string? Country { get; init; }
    public string? City { get; init; }
    public string? ProvinceState { get; init; }

}