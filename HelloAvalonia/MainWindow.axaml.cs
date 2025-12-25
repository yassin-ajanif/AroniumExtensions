using Avalonia.Controls;
using Avalonia.Input;
using HelloAvalonia.ViewModels;

namespace HelloAvalonia;

public partial class MainWindow : Window
{
    public MainWindow() : this(new MainWindowViewModel())
    {
    }

    public MainWindow(MainWindowViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
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

