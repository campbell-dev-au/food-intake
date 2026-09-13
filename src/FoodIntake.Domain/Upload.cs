namespace FoodIntake.Domain;

public class Upload
{
  public int Id { get; private set; }
  public int TimePointId { get; init; }
  public required TimePoint TimePoint { get; init; }
  public string FileName { get; init; } = "";
  public DateTime ImportedAt { get; init; }
  public int LineCount { get; init; }
}
