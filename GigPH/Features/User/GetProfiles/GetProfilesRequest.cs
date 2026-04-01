namespace GigPH.Features.User.GetProfileById;

public record GetProfilesRequest(Guid userId, bool IncludeSocialLinks = false);