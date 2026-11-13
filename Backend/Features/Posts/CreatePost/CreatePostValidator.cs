using FluentValidation;

namespace PrjA.Features.Posts.CreatePost;

public class CreatePostValidator : AbstractValidator<CreatePostRequest>
{
    public CreatePostValidator()
    {
        // TODO: Add validation rules for post content and media.
    }
}

