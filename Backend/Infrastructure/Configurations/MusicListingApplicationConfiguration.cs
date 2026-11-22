using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PrjA.Domain.Entities;

namespace PrjA.Infrastructure.Configurations;

public class MusicListingApplicationConfiguration : IEntityTypeConfiguration<MusicListingApplication>
{
    public void Configure(EntityTypeBuilder<MusicListingApplication> builder)
    {
        builder.HasOne(mla => mla.MusicListingSlot).WithMany(s => s.Applications).HasForeignKey(mla => mla.MusicListingSlotId);
        builder.HasOne(mla => mla.ApplicantUser).WithMany().HasForeignKey(mla => mla.ApplicantUserId);
        

        // HAS ONE one application rows points to one User row only. With many means several appliocation can point to the same user row
        
        
    }
}