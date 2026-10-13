using FoodIntake.Domain;

namespace FoodIntake.App.ViewModels;

public sealed record CategoryChoice(Category? Category)
{
    public static readonly CategoryChoice None = new((Category?)null);

    public string Name => Category?.Name ?? "(none)";
}
