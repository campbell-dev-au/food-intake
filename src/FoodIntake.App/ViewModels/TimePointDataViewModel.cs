using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using FoodIntake.Data;
using FoodIntake.Domain;
using Microsoft.EntityFrameworkCore;

namespace FoodIntake.App.ViewModels;

public sealed record LineRow(
    int LineId,
    string RecordName,
    string DayName,
    string SectionName,
    string FoodName,
    decimal WeightG);

public sealed record NutrientRow(string DisplayName, decimal Value, string Unit);

public partial class TimePointDataViewModel(IDbContextFactory<AppDbContext> dbContextFactory)
: ObservableObject
{
    private readonly IDbContextFactory<AppDbContext> _dbContextFactory = dbContextFactory;

    public ObservableCollection<LineRow> Lines { get; } = [];
    public ObservableCollection<NutrientRow> Nutrients { get; } = [];

    [ObservableProperty]
    public partial string Title { get; set; } = "";

    [ObservableProperty]
    public partial string Summary { get; set; } = "";

    [ObservableProperty]
    public partial LineRow? SelectedLine { get; set; }

    public async Task LoadAsync(int timePointId)
    {
        await using var db = await _dbContextFactory.CreateDbContextAsync();

        TimePoint timePoint = await db.TimePoints
            .Include(t => t.Project)
            .SingleAsync(t => t.Id == timePointId);

        Title = $"{timePoint.Project.Name} – {timePoint.Name}";

        Upload? upload = await db.Uploads.SingleOrDefaultAsync(u => u.TimePointId == timePointId);
        Summary = upload is null
            ? "No data uploaded against this time point."
            : $"{upload.FileName} · imported {upload.ImportedAt.ToLocalTime():yyyy-MM-dd HH:mm} · {upload.LineCount} lines";

        var lines = await db.Lines
            .Where(l => l.FoodRecord.TimePointId == timePointId)
            .OrderBy(l => l.FoodRecordId)
            .ThenBy(l => l.DayName)
            .ThenBy(l => l.MealSectionId)
            .ThenBy(l => l.Id)
            .Select(l => new LineRow(
              l.Id,
              l.FoodRecord.Name == "" ? "(untitled record)" : l.FoodRecord.Name,
              l.DayName,
              l.MealSection.Name,
              l.Food.Name,
              l.WeightG))
            .ToListAsync();

        Lines.Clear();
        foreach (var line in lines)
            Lines.Add(line);
    }

    partial void OnSelectedLineChanged(LineRow? value)
    {
        _ = LoadNutrientsAsync(value);
    }

    private async Task LoadNutrientsAsync(LineRow? line)
    {
        Nutrients.Clear();
        if (line is null)
            return;

        await using var db = await _dbContextFactory.CreateDbContextAsync();

        var nutrients = await db.LineNutrients
            .Where(ln => ln.ConsumptionLineId == line.LineId)
            .OrderBy(ln => ln.NutrientId)
            .Select(ln => new NutrientRow(ln.Nutrient.DisplayName, ln.Value, ln.Nutrient.Unit))
            .ToListAsync();

        foreach (var nutrient in nutrients)
            Nutrients.Add(nutrient);
    }

}
