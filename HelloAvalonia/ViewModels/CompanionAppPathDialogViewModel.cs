using System;
using System.IO;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using AroniumFactures.Services;

namespace AroniumFactures.ViewModels;

public class CompanionAppPathDialogViewModel : ViewModelBase
{
    private string _selectedPath = string.Empty;

    public CompanionAppPathDialogViewModel()
    {
        BrowseCommand = new RelayCommand(async () => await BrowseAsync());
        SubmitCommand = new RelayCommand(() => { }, () => IsValid);
    }

    public RelayCommand BrowseCommand { get; }
    public RelayCommand SubmitCommand { get; }

    public string SelectedPath
    {
        get => _selectedPath;
        set
        {
            if (_selectedPath != value)
            {
                _selectedPath = value ?? string.Empty;
                RaisePropertyChanged();
                UpdateIsValid();
            }
        }
    }

    private bool _isValid;
    public bool IsValid
    {
        get => _isValid;
        set
        {
            if (_isValid != value)
            {
                _isValid = value;
                RaisePropertyChanged();
                SubmitCommand.RaiseCanExecuteChanged();
            }
        }
    }

    private void UpdateIsValid() => IsValid = !string.IsNullOrWhiteSpace(SelectedPath) && File.Exists(SelectedPath);

    private async Task BrowseAsync()
    {
        var window = (Application.Current?.ApplicationLifetime as Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime)?.MainWindow;
        if (window == null) return;

        var files = await window.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Sélectionner l'exécutable Aronium Lite",
            AllowMultiple = false,
            FileTypeFilter = new[]
            {
                new FilePickerFileType("Exécutable")
                {
                    Patterns = new[] { "*.exe" }
                },
                new FilePickerFileType("Tous les fichiers") { Patterns = new[] { "*.*" } }
            }
        });

        if (files.Count > 0)
            SelectedPath = files[0].Path.LocalPath ?? string.Empty;
    }

    public async Task SavePathAsync()
    {
        if (!IsValid) return;
        var settings = await ServiceProvider.LocalSettingsService.LoadSettingsAsync();
        settings.CompanionAppExePath = SelectedPath;
        await ServiceProvider.LocalSettingsService.SaveSettingsAsync(settings);
    }
}
