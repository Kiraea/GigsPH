using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PrjA.Domain.Entities;

namespace PrjA.Infrastructure.Configurations;

public class UserBandConfiguration : IEntityTypeConfiguration<UserBand>
{
    public void Configure(EntityTypeBuilder<UserBand> builder)
    {
        builder.HasKey(x => new {x.UserId, x.BandId});
        
        // if userband has its own key then 
        // hasIndex can work and isunique
        
        // u can use this 
        /*
         * builder.HasOne(x => x.Band)
    .WithMany(band => band.UserBands)
    .HasForeignKey(x => x.BandId)
    .OnDelete(DeleteBehavior.Restrict); 
         this will make it so that it fails if u try to delete the band itself ebcauseu ahve to delete all userband first
         */
    }
}