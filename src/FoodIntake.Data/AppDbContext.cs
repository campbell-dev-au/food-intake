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
    public DbSet<ProjectScheme> ProjectSchemes => Set<ProjectScheme>();

    protected override void OnModelCreating(ModelBuilder model)
    {
        model.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
    }
}
