using GigPH.Features.User.Shared.Dto;

namespace GigPH.Features.User.GetMyProfile;

public record GetMyProfileResponse
{
    public Guid Id { get; init; }
    public string? FirstName { get; init; }
    public string? LastName { get; init; }
    public string? Description { get; init; }
    public List<SocialLinkResponse> SocialLinks { get; init; } = [];
    public List<GenreResponse> Genres{ get; init; } = [];
    public List<InstrumentResponse> Instruments{ get; init; } = [];

    public LocationResponse? Location { get; init; }
}
    
    
    