using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;
using AroniumFactures.Services;

namespace AroniumFactures.ViewModels;

public class ConnectedPhoneItem : ViewModelBase
{
    public int Index { get; init; }
    public string Name { get; init; } = string.Empty;
}

public class MobileConnectionViewModel : ViewModelBase
{
    private string _selectedFolderPath = string.Empty;
    private string _statusMessage = "Sélectionnez un dossier ou un téléphone pour synchroniser.";
    private bool _isSyncing;

    public MobileConnectionViewModel()
    {
        PickFolderCommand = new RelayCommand(async () => await PickFolderAsync());
        SyncCommand = new RelayCommand(async () => await SyncAsync(), () => !string.IsNullOrWhiteSpace(SelectedFolderPath) && !_isSyncing);
        SyncToPhoneCommand = new RelayCommand<ConnectedPhoneItem>(async item => await SyncToPhoneAsync(item), item => item != null && !_isSyncing);
        RefreshPhonesCommand = new RelayCommand(RefreshPhones);

        ConnectedPhones = new ObservableCollection<ConnectedPhoneItem>();
        RefreshPhones();
    }

    public RelayCommand PickFolderCommand { get; }
    public RelayCommand SyncCommand { get; }
    public RelayCommand<ConnectedPhoneItem> SyncToPhoneCommand { get; }
    public RelayCommand RefreshPhonesCommand { get; }

    public ObservableCollection<ConnectedPhoneItem> ConnectedPhones { get; }

    public string SelectedFolderPath
    {
        get => _selectedFolderPath;
        set
        {
            if (_selectedFolderPath == value) return;
            _selectedFolderPath = value ?? string.Empty;
            RaisePropertyChanged();
            SyncCommand.RaiseCanExecuteChanged();
        }
    }

    public string StatusMessage
    {
        get => _statusMessage;
        set
        {
            if (_statusMessage == value) return;
            _statusMessage = value ?? string.Empty;
            RaisePropertyChanged();
        }
    }

    private async Task PickFolderAsync()
    {
        var window = (Avalonia.Application.Current?.ApplicationLifetime as Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime)?.MainWindow;
        if (window?.StorageProvider?.CanPickFolder != true)
        {
            StatusMessage = "Sélection de dossier non disponible.";
            return;
        }

        var folders = await window.StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
        {
            Title = "Choisir le dossier (ex. Téléchargements du téléphone)",
            AllowMultiple = false
        });

        if (folders.Count > 0)
            SelectedFolderPath = folders[0].Path.LocalPath;
    }

    private async Task SyncAsync()
    {
        if (string.IsNullOrWhiteSpace(SelectedFolderPath)) return;

        _isSyncing = true;
        SyncCommand.RaiseCanExecuteChanged();
        StatusMessage = "Synchronisation en cours...";

        try
        {
            var service = ServiceProvider.ManualMobileDesktopSyncingService;
            var (success, message) = await service.SyncToFolderAsync(SelectedFolderPath).ConfigureAwait(true);
            StatusMessage = message;
        }
        catch (Exception ex)
        {
            StatusMessage = "Erreur : " + ex.Message;
        }
        finally
        {
            _isSyncing = false;
            SyncCommand.RaiseCanExecuteChanged();
            SyncToPhoneCommand.RaiseCanExecuteChanged();
        }
    }

    private void RefreshPhones()
    {
        ConnectedPhones.Clear();
        try
        {
            var service = ServiceProvider.ManualMobileDesktopSyncingService;
            var devices = service.GetConnectedDevices();
            foreach (var (index, name) in devices)
                ConnectedPhones.Add(new ConnectedPhoneItem { Index = index, Name = name });
        }
        catch
        {
            // Keep list empty
        }
    }

    private async Task SyncToPhoneAsync(ConnectedPhoneItem? item)
    {
        if (item == null) return;

        _isSyncing = true;
        SyncCommand.RaiseCanExecuteChanged();
        SyncToPhoneCommand.RaiseCanExecuteChanged();
        StatusMessage = "Synchronisation vers " + item.Name + "...";

        try
        {
            var service = ServiceProvider.ManualMobileDesktopSyncingService;
            var (success, message) = await service.SyncToMobileAsync(item.Index).ConfigureAwait(true);
            StatusMessage = message;
        }
        catch (Exception ex)
        {
            StatusMessage = "Erreur : " + ex.Message;
        }
        finally
        {
            _isSyncing = false;
            SyncCommand.RaiseCanExecuteChanged();
            SyncToPhoneCommand.RaiseCanExecuteChanged();
        }
    }
}
