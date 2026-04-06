namespace GigPH.Features.User.UpdateProfile;

public record UpdateProfileRequest()
{
    public Guid UserId { get; set; }
    public string? Description { get; init; }
    public List<Guid> InstrumentIds { get; init; } = [];
    public List<Guid> GenreIds { get; init; } = [];
    public List<string> SocialLinks { get; init; } = [];

}