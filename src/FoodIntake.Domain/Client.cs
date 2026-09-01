namespace FoodIntake.Domain;

public class Client
{
    public int Id { get; private set; }
    public string RemoteClientId { get; init; } = "";
    public string Name { get; set; } = "";
}
