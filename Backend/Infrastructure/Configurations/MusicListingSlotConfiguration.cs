using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PrjA.Domain.Entities;

namespace PrjA.Infrastructure.Configurations;

public class MusicListingSlotConfiguration : IEntityTypeConfiguration<MusicListingSlot>
{
    public void Configure(EntityTypeBuilder<MusicListingSlot> builder)
    {
        builder.HasOne(mls => mls.MusicListing).WithMany(ml => ml.Slots).HasForeignKey(mls => mls.MusicListingId);
        builder.HasOne(mls => mls.Instrument).WithMany().HasForeignKey(mls => mls.InstrumentId).OnDelete(DeleteBehavior.Restrict);
        
        
        
    }
}