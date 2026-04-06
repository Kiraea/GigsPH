namespace GigPH.Domain;

public class Media : BaseEntity
{
    public Guid Id { get; set; } = Guid.CreateVersion7();
    public Guid OwnerId { get; set; }
    public OwnerType OwnerType { get; set; } 
    
    public string Key { get; set; }
    public string Name { get; set; }
    public string Type { get; set; }
    public long FileSize { get; set; }
}