using Microsoft.EntityFrameworkCore;

using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PrjA.Domain.Entities;

namespace PrjA.Infrastructure.Configurations;

public class UserInstrumentScoreConfiguration : IEntityTypeConfiguration<UserInstrumentScore>
{
    public void Configure(EntityTypeBuilder<UserInstrumentScore> builder)
    {
        builder.HasKey(x => new { x.UserId, x.InstrumentId });
    }
}
