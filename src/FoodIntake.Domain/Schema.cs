namespace FoodIntake.Domain;

public class Scheme
{
    public int Id { get; private set; }
    public string Name { get; init; } = "";
    public bool IsBuiltIn { get; init; }
}
