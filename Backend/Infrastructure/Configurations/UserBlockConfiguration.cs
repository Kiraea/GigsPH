using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PrjA.Domain.Entities;

namespace PrjA.Infrastructure.Configurations;

public class UserBlockConfiguration : IEntityTypeConfiguration<UserBlock>
{
    public void Configure(EntityTypeBuilder<UserBlock> builder)
    {
        builder.HasKey(x => new { x.BlockerUserId, x.BlockedUserId });
        builder.HasOne<AppUser>().WithMany().HasForeignKey(x => x.BlockerUserId);
        builder.HasOne<AppUser>().WithMany().HasForeignKey(x => x.BlockedUserId);
    }
}