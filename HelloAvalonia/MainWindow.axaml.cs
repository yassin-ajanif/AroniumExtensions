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
        
        // Set title with version dynamically
        Title = $"Aronium Factures {VersionService.GetInstalledVersion()}";
        
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

        Closing += (_, e) => e.Cancel = true;
    }

    private void MainGrid_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        // ICE TextBox focus logic is now in FactureView (UserControl)
    }
}

