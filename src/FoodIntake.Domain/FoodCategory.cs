namespace FoodIntake.Domain;

public class FoodCategory
{
    public int ProjectId { get; init; }
    public Project? Project { get; init; }
    public int FoodId { get; init; }
    public Food? Food { get; init; }
    public int CategoryId { get; init; }
    public Category? Category { get; init; }
    public int SchemeId { get; init; }
    public string AssignedBy { get; init; } = "";
    public decimal? Confidence { get; init; }
    public DateTime? ConfirmedAt { get; init; }
}
