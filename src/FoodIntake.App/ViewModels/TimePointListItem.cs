using CommunityToolkit.Mvvm.ComponentModel;
using FoodIntake.Domain;

namespace FoodIntake.App.ViewModels;

public partial class TimePointListItem(TimePoint timePoint) : ObservableObject
{
    public TimePoint TimePoint { get; } = timePoint;

    public int Id => TimePoint.Id;
    public string Name => TimePoint.Name;

    [ObservableProperty]
    public partial Upload? Upload { get; set; }

    public bool HasUpload => Upload is not null;

    public string Status => Upload is null
        ? "no data"
        : $"{Upload.LineCount} lines · {Upload.FileName}";

    partial void OnUploadChanged(Upload? value)
    {
        OnPropertyChanged(nameof(HasUpload));
        OnPropertyChanged(nameof(Status));
    }
}
