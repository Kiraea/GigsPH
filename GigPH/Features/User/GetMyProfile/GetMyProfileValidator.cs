using FluentValidation;

namespace GigPH.Features.User.GetMyProfile;

public class GetMyProfileValidator : AbstractValidator<GetMyProfileRequest>
{
    public GetMyProfileValidator()
    {
    }
}