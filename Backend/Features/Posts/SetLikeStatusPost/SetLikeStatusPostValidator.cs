using FluentValidation;

namespace PrjA.Features.Posts.SetLikeStatusPost;

public class SetLikeStatusPostValidator : AbstractValidator<SetLikeStatusPostRequest>
{
    public SetLikeStatusPostValidator()
    {
        // Both true (like) and false (unlike) are valid; IsLiked must be supplied.
    }
}

