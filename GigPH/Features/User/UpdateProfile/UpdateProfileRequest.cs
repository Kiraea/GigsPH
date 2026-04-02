namespace GigPH.Features.User.UpdateProfile;

public record UpdateProfileRequest(Guid UserId, string? DisplayName, string? FirstName, string? LastName, string? Description);