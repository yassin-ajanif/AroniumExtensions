using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using System.Globalization;
using System.Threading;
using AroniumFactures.ViewModels;
using AroniumFactures.Services;

namespace AroniumFactures;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("fr-FR");
        Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("fr-FR");

        // Ensure aronium Extensions folder structure exists (creates if needed)
        DatabaseLocationConfigurationService.EnsureFolderStructure();

        // Get the main database path from Local AppData
        string mainDbPath = DatabaseLocationConfigurationService.GetMainDatabasePath();

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var mainViewModel = new MainWindowViewModel();
            var mainWindow = new MainWindow(mainViewModel);
            desktop.MainWindow = mainWindow;

            // Kick off database + service initialization without blocking UI
            _ = mainViewModel.InitializeAsync(mainDbPath);
        }  

        base.OnFrameworkInitializationCompleted();
    }
}


