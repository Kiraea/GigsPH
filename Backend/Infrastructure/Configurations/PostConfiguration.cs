using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PrjA.Domain.Entities;

namespace PrjA.Infrastructure.Configurations;

public class PostConfiguration : IEntityTypeConfiguration<Post>
{
    public void Configure(EntityTypeBuilder<Post> builder)
    {
        builder.HasOne(p => p.User).WithMany().HasForeignKey(p => p.UserId);
        builder.HasOne<AppUser>().WithMany().HasForeignKey(p => p.CreatedByUserId);
        builder.HasOne(p => p.Band).WithMany().HasForeignKey(p => p.BandId);
        
        
        
        // if one side has a collection and other side has no collection use this
        builder.HasMany(x => x.Genres).WithMany();
        builder.HasMany(x => x.Instruments).WithMany();
    }
}