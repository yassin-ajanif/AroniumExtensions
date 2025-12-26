using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using AroniumFactures.ViewModels;

namespace AroniumFactures;

public partial class MainWindow : Window
{
    public MainWindow() : this(new MainWindowViewModel())
    {
    }

    public MainWindow(MainWindowViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
        
        // Load icon programmatically - this ensures proper loading
        try
        {
            using var stream = AssetLoader.Open(new System.Uri("avares://AroniumFactures/Assets/aronium Facture.png"));
            var bitmap = new Bitmap(stream);
            this.Icon = new WindowIcon(bitmap);
            
        }
        catch
        {
            // If programmatic loading fails, try XAML path
            try
            {
                this.Icon = new WindowIcon("avares://AroniumFactures/Assets/aronium Facture.png");
            }
            catch
            {
                // Icon loading failed, continue without icon
            }
        }
    }

    private void MainGrid_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        // Remove focus from ICE TextBox when clicking outside
        if (IceNumberTextBox != null && IceNumberTextBox.IsFocused)
        {
            // Check if the click was on the ICE TextBox itself
            var source = e.Source;
            if (source != IceNumberTextBox && !IsDescendantOf(IceNumberTextBox, source as Control))
            {
                IceNumberTextBox.IsEnabled = false;
                IceNumberTextBox.IsEnabled = true; // Re-enable to remove focus
            }
        }
    }

    private bool IsDescendantOf(Control ancestor, Control? descendant)
    {
        if (descendant == null) return false;
        var parent = descendant.Parent;
        while (parent != null)
        {
            if (parent == ancestor) return true;
            parent = parent.Parent;
        }
        return false;
    }
}

