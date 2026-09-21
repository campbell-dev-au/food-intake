using FoodIntake.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FoodIntake.Data.Configurations;

internal sealed class NutrientConfiguration : IEntityTypeConfiguration<Nutrient>
{
    public void Configure(EntityTypeBuilder<Nutrient> builder)
    {
        builder.HasIndex(n => n.Code).IsUnique();
    }
}
