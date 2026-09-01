namespace FoodIntake.Domain;

public class Food
{
    public int Id { get; private set; }
    public string DataSourceId { get; init; } = "";
    public string ExternalFoodId { get; init; } = "";
    public string Name { get; set; } = "";
    public ICollection<FoodCategory> Categories { get; init; } = new List<FoodCategory>();
}
