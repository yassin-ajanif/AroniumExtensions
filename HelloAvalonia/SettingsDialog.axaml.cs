using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using AroniumFactures.ViewModels;

namespace AroniumFactures;

public partial class SettingsDialog : Window
{
    public SettingsDialog()
    {
        InitializeComponent();
        DataContext = new SettingsDialogViewModel();
        
        // Load current path
        Loaded += async (s, e) =>
        {
            if (DataContext is SettingsDialogViewModel vm)
            {
                await vm.LoadCurrentPathAsync();
            }
        };
    }

    private void Cancel_Click(object sender, RoutedEventArgs e)
    {
        Close(false);
    }

    private void Save_Click(object sender, RoutedEventArgs e)
    {
        Close(true);
    }
}















































