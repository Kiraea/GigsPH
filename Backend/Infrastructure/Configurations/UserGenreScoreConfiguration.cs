using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PrjA.Domain.Entities;

namespace PrjA.Infrastructure.Configurations;

public class UserGenreScoreConfiguration : IEntityTypeConfiguration<UserGenreScore>
{
    public void Configure(EntityTypeBuilder<UserGenreScore> builder)
    {
        builder.HasKey(x => new { x.UserId, x.GenreId});
    }
}
