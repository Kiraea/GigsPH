using FluentValidation;

namespace PrjA.Features.Posts.DeletePost;

public class DeletePostValidator : AbstractValidator<DeletePostRequest>
{
    public DeletePostValidator()
    {
        RuleFor(x => x.PostId).NotEmpty();
    }
}

