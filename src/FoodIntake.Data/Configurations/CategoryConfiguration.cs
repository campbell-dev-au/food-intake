using FoodIntake.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FoodIntake.Data.Configurations;

internal sealed class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.HasOne(c => c.Parent)
            .WithMany()
            .HasForeignKey(c => c.ParentId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(c => new { c.ParentId, c.Name })
            .IsUnique();

        builder.HasIndex(c => new { c.SchemeId, c.Name })
            .IsUnique()
            .HasFilter("\"ParentId\" IS NULL");
    }
}
