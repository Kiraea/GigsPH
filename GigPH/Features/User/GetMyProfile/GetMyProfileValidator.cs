using FluentValidation;

namespace GigPH.Features.User.GetProfileById;

public class GetMyProfileValidator : AbstractValidator<GetMyProfileRequest>
{
    public GetMyProfileValidator()
    {
    }
}