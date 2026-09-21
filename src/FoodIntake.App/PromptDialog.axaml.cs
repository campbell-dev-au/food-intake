namespace FoodIntake.App;

using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;

public partial class PromptDialog : Window
{
    public PromptDialog()
    {
        InitializeComponent();
    }

    public static Task<string?> AskAsync(Window owner, string message, string initialValue = "")
    {
        PromptDialog dialog = new();
        dialog.MessageText.Text = message;
        dialog.ValueBox.Text = initialValue;

        return dialog.ShowDialog<string?>(owner);
    }

    private void OnCancelClick(object? sender, RoutedEventArgs e) => Close(null);

    private void OnOkClick(object? sender, RoutedEventArgs e)
    {
        string value = ValueBox.Text?.Trim() ?? "";
        Close(value.Length == 0 ? null : value);
    }
}
