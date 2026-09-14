using FoodIntake.Data;
using FoodIntake.Domain;
using Microsoft.EntityFrameworkCore;

namespace FoodIntake.Import;

public sealed record ImportSummary(int RecordCount, int LineCount, int NutrientCount);

public static class FoodWorksImporter
{
    public static async Task<ImportSummary> ImportAsync(
        AppDbContext db,
        int timePointId,
        string filePath,
        CancellationToken cancellationToken = default)
    {
        if (await db.Uploads.AnyAsync(u => u.TimePointId == timePointId, cancellationToken))
        {
            throw new InvalidOperationException("This time point already has an upload. Clear it before uploading again.");
        }

        TimePoint timePoint = await db.TimePoints.SingleAsync(t => t.Id == timePointId, cancellationToken);
        Project project = await db.Projects.SingleAsync(p => p.Id == timePoint.ProjectId, cancellationToken);

        IReadOnlyList<FoodWorksLine> lines = FoodWorksCsvParser.Parse(filePath);

        await GuardAgainstResourcesAlreadyInProjectAsync(db, project.Id, lines, cancellationToken);

        // Build local in-memory stores of data, for existence checking before creating new records
        List<string> clientRemoteIds = [.. lines.Select(l => l.ClientRemoteId).Distinct()];
        Dictionary<string, Client> clients = await db.Clients
            .Where(c => clientRemoteIds.Contains(c.RemoteClientId))
            .ToDictionaryAsync(c => c.RemoteClientId, cancellationToken);

        List<string> sectionNames = [.. lines.Select(l => l.SectionName).Distinct()];
        Dictionary<string, MealSection> sections = await db.MealSections
            .Where(s => sectionNames.Contains(s.Name))
            .ToDictionaryAsync(s => s.Name, cancellationToken);

        List<string> nutrientCodes = [.. lines
            .SelectMany(l => l.Nutrients.Select(n => n.Column.Code))
            .Distinct()];
        Dictionary<string, Nutrient> nutrients = await db.Nutrients
            .Where(n => nutrientCodes.Contains(n.Code))
            .ToDictionaryAsync(n => n.Code, cancellationToken);

        List<string> dataSourceIds = [.. lines.Select(l => l.DataSourceId).Distinct()];
        List<string> foodRemoteIds = [.. lines.Select(l => l.FoodRemoteId).Distinct().ToList()];

        Dictionary<(string DataSourceId, string ExternalFoodId), Food> foods = await db.Foods
            .Where(f => dataSourceIds.Contains(f.DataSourceId) && foodRemoteIds.Contains(f.ExternalFoodId))
            .ToDictionaryAsync(f => (f.DataSourceId, f.ExternalFoodId), cancellationToken);

        HashSet<int> linkedClientIds = await db.ProjectClients
            .Where(pc => pc.ProjectId == project.Id)
            .Select(pc => pc.ClientId)
            .ToHashSetAsync(cancellationToken);

        // begin processing lines from import file
        var foodRecords = new Dictionary<string, FoodRecord>();
        var nutrientCount = 0;

        foreach (FoodWorksLine line in lines)
        {
            if (!clients.TryGetValue(line.ClientRemoteId, out var client))
            {
                client = new Client { RemoteClientId = line.ClientRemoteId, Name = line.ClientName };
                db.Clients.Add(client);
                clients[line.ClientRemoteId] = client;
            }

            if (!linkedClientIds.Contains(client.Id))
            {
                db.ProjectClients.Add(new ProjectClient { Project = project, Client = client });
                linkedClientIds.Add(client.Id);
            }

            if (!foodRecords.TryGetValue(line.ResourceRemoteId, out var record))
            {
                record = new FoodRecord
                {
                    ProjectId = project.Id,
                    Client = client,
                    TimePoint = timePoint,
                    RemoteResourceId = line.ResourceRemoteId,
                    Name = line.ResourceName
                };
                db.FoodRecords.Add(record);
                foodRecords[line.ResourceRemoteId] = record;
            }

            if (!sections.TryGetValue(line.SectionName, out var section))
            {
                section = new MealSection { Name = line.SectionName };
                db.MealSections.Add(section);
                sections[line.SectionName] = section;
            }

            if (!foods.TryGetValue((line.DataSourceId, line.FoodRemoteId), out var food))
            {
                food = new Food
                {
                    DataSourceId = line.DataSourceId,
                    ExternalFoodId = line.FoodRemoteId,
                    Name = line.FoodName
                };
                db.Foods.Add(food);
                foods[(line.DataSourceId, line.FoodRemoteId)] = food;
            }

            var consumptionLine = new ConsumptionLine
            {
                Food = food,
                FoodRecord = record,
                MealSection = section,
                DayName = line.DayName,
                DayDate = line.DayDate,
                WeightG = line.WeightG
            };
            db.Lines.Add(consumptionLine);

            foreach (var value in line.Nutrients)
            {
                if (!nutrients.TryGetValue(value.Column.Code, out var nutrient))
                {
                    nutrient = new Nutrient
                    {
                        Code = value.Column.Code,
                        DisplayName = value.Column.DisplayName,
                        Unit = value.Column.Unit
                    };
                    db.Nutrients.Add(nutrient);
                    nutrients[value.Column.Code] = nutrient;
                }

                consumptionLine.Nutrients.Add(new LineNutrient
                {
                    ConsumptionLine = consumptionLine,
                    Nutrient = nutrient,
                    Value = value.Value
                });
                nutrientCount++;
            }
        }

        // add a record representing the upload itself
        db.Uploads.Add(new Upload
        {
            TimePoint = timePoint,
            FileName = Path.GetFileName(filePath),
            ImportedAt = DateTime.UtcNow,
            LineCount = lines.Count
        });

        await db.SaveChangesAsync(cancellationToken);

        return new ImportSummary(foodRecords.Count, lines.Count, nutrientCount);
    }

    private static async Task GuardAgainstResourcesAlreadyInProjectAsync(
        AppDbContext db,
        int projectId,
        IReadOnlyList<FoodWorksLine> lines,
        CancellationToken cancellationToken
    )
    {
        List<string> resourceIds = [.. lines.Select(l => l.ResourceRemoteId).Distinct()];

        var clashes = await db.FoodRecords
            .Where(r => r.ProjectId == projectId && resourceIds.Contains(r.RemoteResourceId))
            .Select(r => new { r.Name, TimePointName = r.TimePoint.Name })
            .ToListAsync(cancellationToken);

        if (clashes.Count == 0)
            return;

        string detail = string.Join(", ", clashes
            .Select(c => $"'{(c.Name == "" ? "(untitled record)" : c.Name)}' in {c.TimePointName}")
            .Distinct()
            .Take(5));

        throw new InvalidOperationException(
            $"{clashes.Count} food record(s) in this file are already imported in this project: {detail}. " +
            "A food record belongs to one time point — export only the records for this time point, " +
            "or clear the time point that already has them.");
    }
}
