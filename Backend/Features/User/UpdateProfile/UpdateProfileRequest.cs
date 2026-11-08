using PrjA.Domain;

namespace PrjA.Features.User.UpdateProfile;

public class UpdateProfileRequest
{
    public string? DisplayName { get; set; }
    public string? Description { get; set; }

    public List<Guid>? GenreIds { get; set; }
    public List<ProfileInstrumentRequestDto>? Instruments { get; set; }

    public List<ProfileSocialLinkRequestDto>? SocialLinks { get; set; }

    public ProfileLocationRequestDto? Location { get; set; }
}