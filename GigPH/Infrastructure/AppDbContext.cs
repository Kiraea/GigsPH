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

    public DbSet<AppUser> AppUsers { get; set; }
    public DbSet<BandUser> BandUsers{ get; set; }
    public DbSet<Band> Bands{ get; set; }
    public DbSet<Post> Posts{ get; set; }
    public DbSet<Media> Medias{ get; set; }
}