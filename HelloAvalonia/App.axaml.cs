using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Text.Json;
using AroniumFactures.ViewModels;



namespace AroniumFactures;

public partial class App : Application
{

    private const string DefaultMainDbPath = @"C:\ProgramData\Aronium\SimplePos\pos.db";
    
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("fr-FR");
        Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo("fr-FR");

        string mainDbPath = GetLocationOfMainDatabaseApplication();

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


    // Returns the main database location defined in database/location.json, falling back to the default when missing or invalid.
    private string GetLocationOfMainDatabaseApplication()
    {
        try
        {
            string basePath = AppContext.BaseDirectory;
            string locationFile = Path.Combine(basePath, "database", "location.json");
            if (!File.Exists(locationFile))
            {
                return DefaultMainDbPath;
            }

            using FileStream fileStream = File.OpenRead(locationFile);
            using JsonDocument doc = JsonDocument.Parse(fileStream);

            if (doc.RootElement.TryGetProperty("mainDatabasePath", out JsonElement pathElement))
            {
                string? configuredPath = pathElement.GetString();

                if (!string.IsNullOrWhiteSpace(configuredPath))
                {
                    return configuredPath;
                }
            }
        }
        catch
        {
            // Ignore and fall back to default
        }

        return DefaultMainDbPath;
    }
}


