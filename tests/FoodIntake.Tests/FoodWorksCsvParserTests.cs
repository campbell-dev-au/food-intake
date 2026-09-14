using FoodIntake.Import;

namespace FoodIntake.Tests;

public class FoodWorksCsvParserTests
{
    private static string SamplePath =>
        Path.Combine(AppContext.BaseDirectory, "TestData", "ResourcesDetails.csv");

    [Fact]
    public void Parses_every_line_in_the_sample_export()
    {
        IReadOnlyList<FoodWorksLine> lines = FoodWorksCsvParser.Parse(SamplePath);

        Assert.Equal(20, lines.Count);
        Assert.Equal(2, lines.Select(l => l.ResourceRemoteId).Distinct().Count());
        Assert.Single(lines.Select(l => l.ClientRemoteId).Distinct());
    }

    [Fact]
    public void Reads_the_metadata_on_the_first_line()
    {
        FoodWorksLine line = FoodWorksCsvParser.Parse(SamplePath)[0];

        Assert.Equal("Jane Doe", line.ClientName);
        Assert.Equal("Jane Record 1", line.ResourceName);
        Assert.Equal("Day 1", line.DayName);
        Assert.Null(line.DayDate);
        Assert.Equal("Breakfast", line.SectionName);
        Assert.Equal("Woolworths Select Mixed Wholegrain Toast", line.FoodName);
        Assert.Equal(100m, line.WeightG);
    }

    [Fact]
    public void Keeps_the_nutrient_columns_and_splits_their_units()
    {
        FoodWorksLine line = FoodWorksCsvParser.Parse(SamplePath)[0];

        // 101 value columns in the export, 39 of them FoodWorks food groups (which we drop)
        Assert.Equal(62, line.Nutrients.Count);

        FoodWorksNutrientValue protein = line.Nutrients.Single(n => n.Column.Code == "Protein (g)");
        Assert.Equal("Protein", protein.Column.DisplayName);
        Assert.Equal("g", protein.Column.Unit);
        Assert.Equal(8.50m, protein.Value);
    }

    [Fact]
    public void Treats_the_last_parenthesised_group_as_the_unit()
    {
        FoodWorksLine line = FoodWorksCsvParser.Parse(SamplePath)[0];

        // "EnergyDF (Cal) (Cal) would break a naive "first paren group" split.
        FoodWorksNutrientValue energy = line.Nutrients.Single(n => n.Column.Code == "EnergyDF (Cal) (Cal)");
        Assert.Equal("EnergyDF (Cal)", energy.Column.DisplayName);
        Assert.Equal("Cal", energy.Column.Unit);
    }

    [Fact]
    public void Reads_the_headers_that_are_quoted_because_they_contain_commas()
    {
        FoodWorksLine line = FoodWorksCsvParser.Parse(SamplePath)[0];

        var folate = line.Nutrients.Single(n => n.Column.Code == "Folate,total DFE (\u03bcg)");
        Assert.Equal("Folate,total DFE", folate.Column.DisplayName);
        Assert.Equal("\u03bcg", folate.Column.Unit);
        Assert.Equal(286.00m, folate.Value);

        var tocopherol = line.Nutrients.Single(n => n.Column.Code == "Tocopherol, alpha (mg)");
        Assert.Equal("Tocopherol, alpha", tocopherol.Column.DisplayName);
        Assert.Equal(2.40m, tocopherol.Value);
    }

    [Fact]
    public void Splits_a_unit_off_every_nutrient_column()
    {
        FoodWorksLine line = FoodWorksCsvParser.Parse(SamplePath)[0];

        // Sweeps all 62 at once: a header the regex failed on lands here as an empty
        // unit, and a food-group column that slipped through the filter lands here as
        // an unexpected one. Cheaper than naming every column, and catches both.
        Assert.All(line.Nutrients, n => Assert.NotEmpty(n.Column.Unit));
        Assert.Equal(
            ["%", "Cal", "g", "kJ", "mg", "\u03bcg"],
            line.Nutrients.Select(n => n.Column.Unit).Distinct().Order(StringComparer.Ordinal).ToArray());
    }

    [Fact]
    public void Skips_the_FoodWorks_food_group_columns()
    {
        FoodWorksLine line = FoodWorksCsvParser.Parse(SamplePath)[0];
        var codes = line.Nutrients.Select(n => n.Column.Code).ToList();

        Assert.DoesNotContain("GRAINS (serve)", codes);
        Assert.DoesNotContain("  - Tomatoes (serve)", codes);
        Assert.DoesNotContain("OIL EQUIVALENTS (tsp)", codes);
        Assert.DoesNotContain("ALCOHOLIC DRINKS (sd)", codes);
        Assert.DoesNotContain("UNCLASSIFIED WEIGHT (g)", codes);
        Assert.DoesNotContain("UNCLASSIFIED kJ (kJ)", codes);

        // Caffeine sits *after* that block in the header — make sure the filter is
        // matching on what the columns are, not on where they happen to appear.
        Assert.Contains("Caffeine (mg)", codes);
    }

}
