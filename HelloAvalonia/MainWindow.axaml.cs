using Avalonia.Controls;
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
}
