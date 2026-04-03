namespace GigPH.Features.User.GetProfileById;

public record GetPublicProfileRequest(Guid UserId, bool IncludeSocialLinks = false);