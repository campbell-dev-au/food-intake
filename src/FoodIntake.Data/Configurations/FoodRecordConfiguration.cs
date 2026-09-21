using FoodIntake.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FoodIntake.Data.Configurations;

internal sealed class FoodRecordConfiguration : IEntityTypeConfiguration<FoodRecord>
{
    public void Configure(EntityTypeBuilder<FoodRecord> builder)
    {
        builder.HasIndex(r => new { r.ProjectId, r.RemoteResourceId })
          .IsUnique();
    }
}
