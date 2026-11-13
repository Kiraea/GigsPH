using FluentValidation;

namespace PrjA.Features.Posts.GetPost;

public class GetPostValidator : AbstractValidator<GetPostRequest>
{
    public GetPostValidator()
    {
        RuleFor(x => x.PostId).NotEmpty();
    }
}

