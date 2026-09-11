namespace FoodIntake.Domain;

public class TimePoint
{
    public int Id { get; private set; }
    public int ProjectId { get; init; }
    public required Project Project { get; init; }
    public string Name { get; set; } = "";
    public int SortOrder { get; set; }
}
