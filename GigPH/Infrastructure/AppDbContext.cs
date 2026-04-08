using GigPH.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GigPH.Infrastructure;

public class AppDbContext : IdentityDbContext<AppUser, IdentityRole<Guid>, Guid>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(typeof(Program).Assembly);
        base.OnModelCreating(builder);
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        var entities = ChangeTracker.Entries<IBaseEntity>()
            .Where(e => e.State is EntityState.Modified or EntityState.Added);

        foreach (var entity in entities)
        {
            if (entity.State is EntityState.Modified)
            {
                entity.Entity.ModifiedAt = DateTime.UtcNow;
            }

            if (entity.State is EntityState.Added)
            {
                entity.Entity.CreatedAt = DateTime.UtcNow;
                entity.Entity.ModifiedAt= DateTime.UtcNow;
            }
        }
        return base.SaveChangesAsync(cancellationToken);
    }

    public DbSet<AppUser> AppUsers { get; set; }
    public DbSet<Genre> Genres{ get; set; }
    public DbSet<Instrument> Instruments{ get; set; }
    public DbSet<Band> Bands{ get; set; }
    public DbSet<Post> Posts{ get; set; }
    public DbSet<SocialLink> SocialLinks{ get; set; }
    public DbSet<Media> Medias{ get; set; }
    public DbSet<Location> Locations{ get; set; }
}