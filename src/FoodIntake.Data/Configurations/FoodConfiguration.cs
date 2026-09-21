using FoodIntake.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FoodIntake.Data.Configurations;

internal sealed class FoodConfiguration : IEntityTypeConfiguration<Food>
{
    public void Configure(EntityTypeBuilder<Food> builder)
    {
        builder.HasIndex(f => new { f.DataSourceId, f.ExternalFoodId }).IsUnique();
    }
}
