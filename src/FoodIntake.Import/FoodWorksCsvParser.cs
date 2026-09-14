using System.Globalization;
using System.Text.RegularExpressions;
using CsvHelper;

namespace FoodIntake.Import;

public sealed record FoodWorksNutrientColumn(string Code, string DisplayName, string Unit);
public sealed record FoodWorksNutrientValue(FoodWorksNutrientColumn Column, decimal Value);

public sealed record FoodWorksLine(
    string ClientRemoteId,
    string ClientName,
    string ResourceRemoteId,
    string ResourceName,
    string DayName,
    DateOnly? DayDate,
    string SectionName,
    string DataSourceId,
    string FoodRemoteId,
    string FoodName,
    decimal WeightG,
    IReadOnlyList<FoodWorksNutrientValue> Nutrients
);

public static partial class FoodWorksCsvParser
{
    private const string LastMetadataColumn = "Weight (g)";

    [GeneratedRegex(@"^(?<name>.*\S)\s*\((?<unit>[^()]*)\)$")]
    private static partial Regex UnitSuffix();

    public static IReadOnlyList<FoodWorksLine> Parse(string path)
    {
        using var reader = new StreamReader(path);
        return Parse(reader);
    }

    public static IReadOnlyList<FoodWorksLine> Parse(TextReader reader)
    {
        using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);

        csv.Read();
        csv.ReadHeader();
        string[] header = csv.HeaderRecord
            ?? throw new InvalidDataException("The file has no header row.");

        int lastMetaIndex = Array.IndexOf(header, LastMetadataColumn);
        if (lastMetaIndex < 0)
        {
            throw new InvalidDataException(
                $"Expected a '{LastMetadataColumn}' column. This doesn't look like a FoodWorks ResourcesDetails export."
            );
        }

        var nutrientColumns = new List<(int Index, FoodWorksNutrientColumn Column)>();
        for (var i = lastMetaIndex + 1; i < header.Length; i++)
        {
            FoodWorksNutrientColumn column = ToNutrientColumn(header[i]);
            if (!IsFoodWorksFoodGroup(column))
                nutrientColumns.Add((i, column));
        }

        var lines = new List<FoodWorksLine>();
        while (csv.Read())
        {
            int row = csv.Parser.Row;
            var nutrients = new List<FoodWorksNutrientValue>(nutrientColumns.Count);
            foreach (var (index, column) in nutrientColumns)
            {
                nutrients.Add(new FoodWorksNutrientValue(column, Number(csv.GetField(index), column.Code, row)));
            }

            lines.Add(new FoodWorksLine(
                    ClientRemoteId: Text(csv, "Client ID"),
                    ClientName: Text(csv, "Client Name"),
                    ResourceRemoteId: Text(csv, "Resource ID"),
                    ResourceName: Text(csv, "Resource Name"),
                    DayName: Text(csv, "Day Name"),
                    DayDate: Date(Text(csv, "Day Date"), row),
                    SectionName: Text(csv, "Section Name"),
                    DataSourceId: Text(csv, "Data Source ID"),
                    FoodRemoteId: Text(csv, "Food ID"),
                    FoodName: Text(csv, "Food Name"),
                    WeightG: Number(Text(csv, LastMetadataColumn), LastMetadataColumn, row),
                    Nutrients: nutrients
                ));
        }

        return lines;
    }

    private static FoodWorksNutrientColumn ToNutrientColumn(string header)
    {
        Match match = UnitSuffix().Match(header);
        return match.Success
            ? new FoodWorksNutrientColumn(header, match.Groups["name"].Value, match.Groups["unit"].Value)
            : new FoodWorksNutrientColumn(header, header, "");
    }

    private static bool IsFoodWorksFoodGroup(FoodWorksNutrientColumn column) =>
        column.Unit is "serve" or "tsp" or "sd"
        || column.Code.TrimStart().StartsWith("UNCLASSIFIED", StringComparison.OrdinalIgnoreCase);

    private static string Text(CsvReader csv, string column) => csv.GetField(column)?.Trim() ?? "";

    private static decimal Number(string? raw, string column, int row)
    {
        if (string.IsNullOrWhiteSpace(raw))
            return 0m;

        return decimal.TryParse(raw, NumberStyles.Float, CultureInfo.InvariantCulture, out var value)
            ? value
            : throw new InvalidDataException($"Row {row}: couldn't read '{raw}' in column '{column}' as a number.");
    }

    private static DateOnly? Date(string raw, int row)
    {
        if (string.IsNullOrWhiteSpace(raw))
            return null;

        string[] formats = ["yyyy/MM/dd", "yyyy-MM-dd"];
        return DateOnly.TryParseExact(raw, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out var date)
        ? date
        : throw new InvalidDataException($"Row {row}: couldn't read '{raw}' as a date (expected yyyy/MM/dd).");
    }
}
