using FoodIntake.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FoodIntake.Data.Configurations;

internal sealed class TimePointConfiguration : IEntityTypeConfiguration<TimePoint>
{
    public void Configure(EntityTypeBuilder<TimePoint> builder)
    {
        builder.HasIndex(t => new { t.ProjectId, t.Name }).IsUnique();
    }
}
