using CommunityToolkit.Mvvm.ComponentModel;
using FoodIntake.Domain;

namespace FoodIntake.App.ViewModels;

public partial class SchemeAssignment(Scheme scheme) : ObservableObject
{
    public Scheme Scheme { get; } = scheme;

    public int Id => Scheme.Id;
    public string Name => Scheme.Name;

    [ObservableProperty]
    public partial bool IsAssigned { get; set; }
}
