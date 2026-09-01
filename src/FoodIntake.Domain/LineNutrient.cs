namespace FoodIntake.Domain;

public class LineNutrient
{
    public int ConsumptionLineId { get; init; }
    public required ConsumptionLine ConsumptionLine { get; init; }
    public int NutrientId { get; init; }
    public required Nutrient Nutrient { get; init; }
    public decimal Value { get; init; }
}
