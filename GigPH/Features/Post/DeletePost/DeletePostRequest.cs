namespace GigPH.Features.Post.DeletePost;

public record DeletePostRequest
{
    public Guid UserId { get; set; }
    public Guid PostId { get; set; }
}