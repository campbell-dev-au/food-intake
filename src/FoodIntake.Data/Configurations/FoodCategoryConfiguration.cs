using FoodIntake.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FoodIntake.Data.Configurations;

internal sealed class FoodCategoryConfiguration : IEntityTypeConfiguration<FoodCategory>
{
    public void Configure(EntityTypeBuilder<FoodCategory> builder)
    {
        builder.HasKey(fc => new { fc.ProjectId, fc.FoodId, fc.SchemeId });

        builder.HasOne(fc => fc.Category)
            .WithMany()
            .HasForeignKey(fc => new { fc.CategoryId, fc.SchemeId })
            .HasPrincipalKey(c => new { c.Id, c.SchemeId })
            .OnDelete(DeleteBehavior.Cascade);
    }
}
