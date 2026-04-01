using GigPH.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace GigPH.Infrastructure;

public class AppDbContext : IdentityDbContext<IdentityUser<Guid>,IdentityRole<Guid>, Guid>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(typeof(Program).Assembly);
        base.OnModelCreating(builder);
        
        
    }

    private DbSet<AppUser> AppUsers { get; set; }
    private DbSet<BandUser> BandUsers{ get; set; }
    private DbSet<Band> Band{ get; set; }
}