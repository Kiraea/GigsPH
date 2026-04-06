using GigPH.Features.User.Shared.Dto;
namespace GigPH.Features.User.UpdateProfile;

public record UpdateProfileResponse
{
    public Guid Id { get; init; }
    public string? Description { get; init; }
    public List<SocialLinkResponse> SocialLinks { get; init; } = [];
    public List<BandResponse> Bands{ get; init; } = [];
    public List<GenreResponse> Genres{ get; init; } = [];
    public List<InstrumentResponse> Instruments{ get; init; } = [];
 
}
   
    
    