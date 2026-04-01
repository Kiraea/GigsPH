namespace GigPH.Features.User.GetProfileById;

public record GetProfileByIdRequest(Guid UserId, bool IncludeSocialLinks = false);