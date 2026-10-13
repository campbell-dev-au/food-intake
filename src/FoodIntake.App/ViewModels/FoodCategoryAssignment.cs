using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;

namespace FoodIntake.App.ViewModels;

public sealed record ProjectFood(int FoodId, string Name, int UsageCount);

public partial class FoodCategoryAssignment(
    ProjectFood food,
    IReadOnlyList<CategoryChoice> choices
) : ObservableObject
{
    public ProjectFood Food { get; } = food;

    public IReadOnlyList<CategoryChoice> Choices { get; } = choices;

    public int FoodId => Food.FoodId;
    public string Name => Food.Name;
    public string UsageSummary => Food.UsageCount == 1 ? "1 line" : $"{Food.UsageCount} lines";

    [ObservableProperty]
    public partial CategoryChoice SelectedChoice { get; set; } = CategoryChoice.None;

    public CategoryChoice LastSavedChoice { get; set; } = CategoryChoice.None;
}

