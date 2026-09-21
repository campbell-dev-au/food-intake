using FoodIntake.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FoodIntake.Data.Configurations;

internal sealed class ProjectClientConfiguration : IEntityTypeConfiguration<ProjectClient>
{
    public void Configure(EntityTypeBuilder<ProjectClient> builder)
    {
        builder.HasKey(pc => new { pc.ProjectId, pc.ClientId });
    }
}
