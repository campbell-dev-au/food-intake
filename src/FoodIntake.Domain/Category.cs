namespace FoodIntake.Domain;

public class Category
{
    public int Id { get; private set; }
    public int SchemeId { get; init; }
    public required Scheme Scheme { get; init; }
    public int? ParentId { get; init; }
    public Category? Parent { get; init; }
    public string Name { get; set; } = "";
}
