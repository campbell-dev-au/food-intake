using FoodIntake.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FoodIntake.Data.Configurations;

internal sealed class LineNutrientConfiguration : IEntityTypeConfiguration<LineNutrient>
{
    public void Configure(EntityTypeBuilder<LineNutrient> builder)
    {
        builder.HasKey(ln => new { ln.ConsumptionLineId, ln.NutrientId });
    }
}
