using PrjA.Features.Genres;
using PrjA.Features.Instruments;

namespace PrjA.Features.Posts.GetPublicPosts;

public class GetPublicPostsResponse
{
    
    public ICollection<PostDto> Posts { get; set; } = [];
    public string? Cursor { get; set; }
}

public class PostDto
{
    public Guid Id { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
    
    // can only be one
    public Guid? BandId { get; set; }
    public string? UserId { get; set; }
    public required string PublisherName { get; set; }
    
    public int LikedCount { get; set; }
    public bool IsLiked { get; set; }

    public ICollection<PostMediaDto> Media { get; set; } = [];
    public ICollection<GenreDto> Genres { get; set; } = [];
    public ICollection<InstrumentDto> Instruments { get; set; } = [];
    public DateTime CreatedAt { get; set; }

}

public class PostMediaDto
{
    public Guid Id { get; set; }
    public required string ContentType{ get; set; }
    public required string Url { get; set; }
}

