using GigPH.Features.User.Shared.Dto;

namespace GigPH.Features.User.GetProfileById;

public record GetProfilesResponse
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Description { get; set; }
    public List<SocialLinkResponse>? SocialLinkResponses { get; set; }
}