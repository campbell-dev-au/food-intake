namespace FoodIntake.Domain;

public class FoodRecord
{
    public int Id { get; private set; }
    public int ProjectId { get; init; }
    public int ClientId { get; init; }
    public required Client Client { get; init; }
    public int TimePointId { get; init; }
    public required TimePoint TimePoint { get; init; }
    public string RemoteResourceId { get; init; } = "";
    public string Name { get; set; } = "";
}
