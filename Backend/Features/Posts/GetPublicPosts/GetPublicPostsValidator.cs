using FluentValidation;

namespace PrjA.Features.Posts.GetPublicPosts;

public class GetPublicPostsValidator : AbstractValidator<GetPublicPostsRequest>
{
    public GetPublicPostsValidator()
    {
        RuleFor(x => x.Page).GreaterThan(0);
        RuleFor(x => x.PageSize).InclusiveBetween(1, 100);
    }
}

