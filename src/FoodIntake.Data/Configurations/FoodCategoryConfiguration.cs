using FoodIntake.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FoodIntake.Data.Configurations;

internal sealed class FoodCategoryConfiguration : IEntityTypeConfiguration<FoodCategory>
{
    public void Configure(EntityTypeBuilder<FoodCategory> builder)
    {
        builder.HasKey(fc => new { fc.FoodId, fc.CategoryId });
    }
}
