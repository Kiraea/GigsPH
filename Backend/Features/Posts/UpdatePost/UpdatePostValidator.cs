using FluentValidation;

namespace PrjA.Features.Posts.UpdatePost;

public class UpdatePostValidator : AbstractValidator<UpdatePostRequest>
{
    public UpdatePostValidator()
    {
        // TODO: Add validation rules for post content and media.
    }
}

