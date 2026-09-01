namespace FoodIntake.Domain;

public class ProjectClient
{
    public int ProjectId { get; init; }
    public required Project Project { get; init; }
    public int ClientId { get; init; }
    public required Client Client { get; init; }
}
