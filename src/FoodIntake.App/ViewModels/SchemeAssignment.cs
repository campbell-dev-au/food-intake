using CommunityToolkit.Mvvm.ComponentModel;
using FoodIntake.Domain;

namespace FoodIntake.App.ViewModels;

public partial class SchemeAssignment(Scheme scheme) : ObservableObject
{
    public Scheme Scheme { get; } = scheme;

    public int Id => Scheme.Id;
    public string Name => Scheme.Name;
    public int CategoryCount { get; init; }
    public string CategorySummary => CategoryCount == 1 ? "1 category" : $"{CategoryCount} categories";

    [ObservableProperty]
    public partial bool IsAssigned { get; set; }
}
