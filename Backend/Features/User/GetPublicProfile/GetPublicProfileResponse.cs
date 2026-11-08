using PrjA.Features.User.UpdateProfile;

namespace PrjA.Features.User.GetPublicProfile;

public class GetPublicProfileResponse 
{
    public required string Firstname { get; set; }
    public required string LastName { get; set; }
    public required string DisplayName { get; set; }
    public required string Description { get; set; }

    public ICollection<ProfileGenreResponseDto> Genres { get; set; } = [];
    public ICollection<ProfileInstrumentResponseDto> UserInstruments { get; set; } = [];
    public ICollection<ProfileSocialLinkResponseDto> SocialLinks { get; set; } = [];
    
    public required ProfileLocationResponseDto Location{ get; set; }
}
