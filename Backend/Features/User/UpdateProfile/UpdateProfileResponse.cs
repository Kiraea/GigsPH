namespace PrjA.Features.User.UpdateProfile;

public class UpdateProfileResponse
{
    public required string Id { get; set; }
    public required string DisplayName { get; set; }
    public string? Description { get; set; }

    public List<ProfileGenreResponseDto> Genres { get; set; } = [];
    public List<ProfileInstrumentResponseDto> Instruments { get; set; } = [];
    public List<ProfileSocialLinkResponseDto> SocialLinks { get; set; } = [];
    public required ProfileLocationResponseDto Location { get; set; }
    public bool IsOnboarded { get; set; }
}