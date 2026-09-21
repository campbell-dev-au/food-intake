using FoodIntake.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FoodIntake.Data.Configurations;

internal sealed class ProjectSchemeConfiguration : IEntityTypeConfiguration<ProjectScheme>
{
    public void Configure(EntityTypeBuilder<ProjectScheme> builder)
    {
        builder.HasKey(ps => new { ps.ProjectId, ps.SchemeId });
    }
}
