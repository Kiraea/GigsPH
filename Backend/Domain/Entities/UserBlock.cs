namespace PrjA.Domain.Entities;

public class UserBlock : ITimestamp
{
    public Guid Id { get; set; }
    public required string BlockerUserId { get; set; }
    public required string BlockedUserId { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime CreatedAt { get; set; }
}