namespace FoodIntake.Domain;

public class FoodCategory
{
    public int FoodId { get; init; }
    public required Food Food { get; init; }
    public int CategoryId { get; init; }
    public required Category Category { get; init; }
    public string AssignedBy { get; init; } = "";
    public decimal? Confidence { get; init; }
    public DateTime? ConfirmedAt { get; init; }
}
