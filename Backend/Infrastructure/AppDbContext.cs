using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PrjA.Domain.Entities;

namespace PrjA.Infrastructure;

public class AppDbContext : IdentityDbContext<AppUser>
{
    public AppDbContext(DbContextOptions options) : base(options)
    {

    }
    
    //onModelCreating
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<UserInstrument>().HasKey(x => new {x.AppUserId, x.InstrumentId});
    } 

    public DbSet<AppUser> AppUsers { get; set; }
    public DbSet<Band> Bands { get; set; }
    public DbSet<Genre> Genres { get; set; }
    public DbSet<Instrument> Instruments { get; set; }
    public DbSet<Location> Locations { get; set; }
    public DbSet<SocialLink> SocialLinks { get; set; }
}
