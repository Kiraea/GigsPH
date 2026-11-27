namespace PrjA.Domain.Entities;

public class Post : ITimestamp
{
    public Guid Id { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }

    public ICollection<PostLike> Likes { get; set; } = [];
    public ICollection<MediaUpload> MediaUploads { get; set; } = [];
    
    
    
    public ICollection<Instrument> Instruments { get; set; } = [];
    public ICollection<Genre> Genres{ get; set; } = [];
    
    
    
    // can only be either 
    public string? UserId { get; set; }
    public AppUser? User { get; set; }
    public Guid? BandId { get; set; }
    
    public Band? Band{ get; set; } 
    public required string CreatedByUserId { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime CreatedAt { get; set; }
}