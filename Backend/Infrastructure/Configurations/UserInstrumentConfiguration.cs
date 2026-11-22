using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PrjA.Domain.Entities;

namespace PrjA.Infrastructure.Configurations;

public class UserInstrumentConfiguration : IEntityTypeConfiguration<UserInstrument>
{
    public void Configure(EntityTypeBuilder<UserInstrument> builder)
    {
        builder.HasKey(x => new {x.InstrumentId, x.AppUserId});
        
    }
}