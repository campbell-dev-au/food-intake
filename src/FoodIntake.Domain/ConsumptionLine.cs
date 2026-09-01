namespace FoodIntake.Domain;

public class ConsumptionLine
{
    public int Id { get; private set; }
    public int FoodId { get; init; }
    public required Food Food { get; init; }
    public int FoodRecordId { get; init; }
    public required FoodRecord FoodRecord { get; init; }
    public string DayName { get; set; } = "";
    public DateOnly? DayDate { get; init; }
    public int MealSectionId { get; init; }
    public required MealSection MealSection { get; init; }
    public decimal WeightG { get; init; }
    public ICollection<LineNutrient> Nutrients { get; init; } = new List<LineNutrient>();
}
