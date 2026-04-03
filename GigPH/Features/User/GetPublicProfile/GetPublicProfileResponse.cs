using GigPH.Features.User.Shared.Dto;

namespace GigPH.Features.User.GetProfileById;

public record GetPublicProfileResponse(
    Guid Id,
    string FirstName,
    string LastName,
    string Description, 
    List<SocialLinkResponse>? SocialLinks);
    
    
    