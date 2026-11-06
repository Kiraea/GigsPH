namespace PrjA.Domain.Entities;

public interface IBaseEntity
{
    public DateTime CreatedAt { get; set;  }
    public DateTime UpdatedAt { get; set; }
    
}

public abstract class BaseEntity : IBaseEntity
{
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}