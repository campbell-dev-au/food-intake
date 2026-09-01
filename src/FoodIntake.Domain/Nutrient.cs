namespace FoodIntake.Domain;

public class Nutrient
{
    public int Id { get; private set; }
    public string Code { get; init; } = "";
    public string DisplayName { get; init; } = "";
    public string Unit { get; init; } = "";
}
