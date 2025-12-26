using System;
using System.IO;
using System.Threading.Tasks;
using System.Text.Json;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using AroniumFactures.Models;

namespace AroniumFactures.ViewModels;

public class SettingsDialogViewModel : ViewModelBase
{
    private string _databasePath = string.Empty;
    private bool _isValid;
    private string _statusMessage = string.Empty;

    public SettingsDialogViewModel()
    {
        BrowseCommand = new RelayCommand(async () => await BrowseForDatabaseAsync());
        SaveCommand = new RelayCommand(async () => await SaveSettingsAsync(), () => IsValid);
        TestConnectionCommand = new RelayCommand(async () =>  TestConnection());
    }

    public RelayCommand BrowseCommand { get; }
    public RelayCommand SaveCommand { get; }
    public RelayCommand TestConnectionCommand { get; }

    public string DatabasePath
    {
        get => _databasePath;
        set
        {
            if (_databasePath != value)
            {
                _databasePath = value;
                RaisePropertyChanged();
                ValidatePath();
            }
        }
    }

    public bool IsValid
    {
        get => _isValid;
        set
        {
            if (_isValid != value)
            {
                _isValid = value;
                RaisePropertyChanged();
                SaveCommand.RaiseCanExecuteChanged();
            }
        }
    }

    public string StatusMessage
    {
        get => _statusMessage;
        set
        {
            if (_statusMessage != value)
            {
                _statusMessage = value;
                RaisePropertyChanged();
            }
        }
    }

    private async Task BrowseForDatabaseAsync()
    {
        var window = (Avalonia.Application.Current?.ApplicationLifetime as Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime)?.MainWindow;
        if (window == null) return;

        var files = await window.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Selectionner la base de donnees",
            AllowMultiple = false,
            FileTypeFilter = new[]
            {
                new FilePickerFileType("Base de donnees SQLite")
                {
                    Patterns = new[] { "*.db", "*.sqlite", "*.db3" }
                },
                new FilePickerFileType("Tous les fichiers")
                {
                    Patterns = new[] { "*.*" }
                }
            }
        });

        if (files.Count > 0)
        {
            DatabasePath = files[0].Path.LocalPath;
        }
    }

    private void ValidatePath()
    {
        if (string.IsNullOrWhiteSpace(DatabasePath))
        {
            IsValid = false;
            StatusMessage = "Veuillez selectionner une base de donnees";
            return;
        }

        if (!File.Exists(DatabasePath))
        {
            IsValid = false;
            StatusMessage = "Fichier introuvable";
            return;
        }

        IsValid = true;
        StatusMessage = "Fichier valide";
    }

    private void TestConnection()
    {
        StatusMessage = "Test de connexion...";

        try
        {
            using var connection = new Microsoft.Data.Sqlite.SqliteConnection($"Data Source={DatabasePath}");
            connection.Open();

            using var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT COUNT(*) FROM Document";
            var scalar = cmd.ExecuteScalar();
            var count = scalar is long l ? l : Convert.ToInt64(scalar ?? 0);

            StatusMessage = $"Connexion reussie! ({count} factures trouvees)";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Erreur: {ex.Message}";
        }
    }

    private async Task SaveSettingsAsync()
    {
        try
        {
            // Ensure directory exists
            Directory.CreateDirectory("database");
            
            // Save to JSON settings file
            var settings = new SettingsData { MainDatabasePath = DatabasePath };
            var options = new JsonSerializerOptions { WriteIndented = true };
            var json = JsonSerializer.Serialize(settings, options);
            await File.WriteAllTextAsync("database/settings.json", json);
            
            StatusMessage = "Parametres enregistres! Redemarrez l'application.";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Erreur: {ex.Message}";
        }
    }

    public async Task LoadCurrentPathAsync()
    {
        try
        {
            const string settingsPath = "database/settings.json";
            
            if (File.Exists(settingsPath))
            {
                var json = await File.ReadAllTextAsync(settingsPath);
                var settings = JsonSerializer.Deserialize<SettingsData>(json);
                if (settings != null && !string.IsNullOrEmpty(settings.MainDatabasePath))
                {
                    DatabasePath = settings.MainDatabasePath;
                    return;
                }
            }
            
            // Default path if file doesn't exist or is invalid
            DatabasePath = @"C:\ProgramData\Aronium\SimplePos\pos.db";
        }
        catch
        {
            DatabasePath = @"C:\ProgramData\Aronium\SimplePos\pos.db";
        }
    }

    private class SettingsData
    {
        public string? MainDatabasePath { get; set; }
    }
}
