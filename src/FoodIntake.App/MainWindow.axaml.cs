using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using FoodIntake.App.ViewModels;

namespace FoodIntake.App;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private ProjectsViewModel? ViewModel => DataContext as ProjectsViewModel;

    private async void OnUploadClick(object? sender, RoutedEventArgs e)
    {
        if (ViewModel is not { } viewModel)
            return;

        var files = await StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Select a FoodWorks ResourcesDetails.csv",
            AllowMultiple = false,
            FileTypeFilter = [new FilePickerFileType("CSV files") { Patterns = ["*.csv"] }]
        });

        if (files.Count == 0)
            return;

        var path = files[0].TryGetLocalPath();
        if (path is null)
            return;

        await viewModel.UploadAsync(path);
    }

    private async void OnViewClick(object? sender, RoutedEventArgs e)
    {
        if (ViewModel is not { } viewModel)
            return;

        var dataViewModel = await viewModel.OpenSelectedTimePointAsync();
        if (dataViewModel is null)
            return;

        var window = new TimePointDataWindow { DataContext = dataViewModel };
        window.Show(this);
    }

    private async void OnClearClick(object? sender, RoutedEventArgs e)
    {
        if (ViewModel is not { SelectedTimePoint.HasUpload: true } viewModel)
            return;

        var confirmed = await ConfirmDialog.AskAsync(this,
            $"Remove all imported data for '{viewModel.SelectedTimePoint!.Name}'? This cannot be undone.");

        if (!confirmed)
            return;

        await viewModel.ClearUploadAsync();
    }

    private async void OnNewProjectClick(object? sender, RoutedEventArgs e)
    {
        if (ViewModel is not { } viewModel)
            return;

        string? name = await PromptDialog.AskAsync(this, "Name for the new project:");
        if (name is null)
            return;

        await viewModel.AddProjectAsync(name);
    }

    private async void OnRenameProjectClick(object? sender, RoutedEventArgs e)
    {
        if (ViewModel is not { SelectedProject: { } project } viewModel)
            return;

        string? name = await PromptDialog.AskAsync(this, "New name:", project.Name);
        if (name is null)
            return;

        await viewModel.RenameProjectAsync(name);
    }
}
