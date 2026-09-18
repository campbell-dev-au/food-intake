using FoodIntake.Domain;
using Microsoft.EntityFrameworkCore;

namespace FoodIntake.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Project> Projects => Set<Project>();
    public DbSet<TimePoint> TimePoints => Set<TimePoint>();
    public DbSet<Client> Clients => Set<Client>();
    public DbSet<ProjectClient> ProjectClients => Set<ProjectClient>();
    public DbSet<FoodRecord> FoodRecords => Set<FoodRecord>();
    public DbSet<Food> Foods => Set<Food>();
    public DbSet<MealSection> MealSections => Set<MealSection>();
    public DbSet<ConsumptionLine> Lines => Set<ConsumptionLine>();
    public DbSet<Nutrient> Nutrients => Set<Nutrient>();
    public DbSet<LineNutrient> LineNutrients => Set<LineNutrient>();
    public DbSet<Scheme> Schemes => Set<Scheme>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<FoodCategory> FoodCategories => Set<FoodCategory>();
    public DbSet<Upload> Uploads => Set<Upload>();

    protected override void OnModelCreating(ModelBuilder model)
    {
        model.Entity<Project>()
          .HasIndex(p => p.Name)
          .IsUnique();

        model.Entity<TimePoint>()
          .HasIndex(t => new { t.ProjectId, t.Name })
          .IsUnique();

        model.Entity<Client>()
          .HasIndex(c => c.RemoteClientId)
          .IsUnique();

        model.Entity<ProjectClient>()
          .HasKey(pc => new { pc.ProjectId, pc.ClientId });

        model.Entity<FoodRecord>()
          .HasIndex(r => new { r.ProjectId, r.RemoteResourceId })
          .IsUnique();

        model.Entity<MealSection>()
          .HasIndex(s => s.Name)
          .IsUnique();

        model.Entity<Food>()
          .HasIndex(f => new { f.DataSourceId, f.ExternalFoodId })
          .IsUnique();

        model.Entity<Nutrient>()
          .HasIndex(n => n.Code)
          .IsUnique();

        model.Entity<LineNutrient>()
          .HasKey(ln => new { ln.ConsumptionLineId, ln.NutrientId });

        model.Entity<Scheme>()
          .HasIndex(s => s.Name)
          .IsUnique();

        model.Entity<Category>()
          .HasOne(c => c.Parent)
          .WithMany()
          .HasForeignKey(c => c.ParentId)
          .OnDelete(DeleteBehavior.Restrict);

        model.Entity<Category>()
          .HasIndex(c => new { c.ParentId, c.Name })
          .IsUnique();

        model.Entity<Category>()
            .HasIndex(c => new { c.SchemeId, c.Name })
            .IsUnique()
            .HasFilter("\"ParentId\" IS NULL");

        model.Entity<FoodCategory>()
          .HasKey(fc => new { fc.FoodId, fc.CategoryId });

        model.Entity<Upload>()
          .HasIndex(u => u.TimePointId)
          .IsUnique();
    }
}
