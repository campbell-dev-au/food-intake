using FoodIntake.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FoodIntake.Data.Configurations;

internal sealed class MealSectionConfiguration : IEntityTypeConfiguration<MealSection>
{
    public void Configure(EntityTypeBuilder<MealSection> builder)
    {
        builder.HasIndex(s => s.Name).IsUnique();
    }
}
