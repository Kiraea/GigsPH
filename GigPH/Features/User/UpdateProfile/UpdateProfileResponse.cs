using GigPH.Features.User.Shared.Dto;
namespace GigPH.Features.User.UpdateProfile;
public record UpdateProfileResponse(
    Guid Id,
    string FirstName,
    string LastName,
    string Description, 
    List<SocialLinkResponse>? SocialLinks);
    
    
    