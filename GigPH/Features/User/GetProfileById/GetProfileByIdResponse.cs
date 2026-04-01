using GigPH.Features.User.Shared.Dto;

namespace GigPH.Features.User.GetProfileById;

public record GetProfileByIdResponse(
    Guid Id,
    string FirstName,
    string LastName,
    string Description, 
    List<SocialLinkResponse>? SocialLinks);
    
    
    