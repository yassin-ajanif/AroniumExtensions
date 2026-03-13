using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;
using AroniumFactures.Services;

namespace AroniumFactures.ViewModels;

public class ConnectedPhoneItem : ViewModelBase
{
    private string _lastStatus = string.Empty;
    private string _syncStatusColor = "#6B7280";

    public int Index { get; init; }
    public string Name { get; init; } = string.Empty;

    public string LastStatus
    {
        get => _lastStatus;
        set
        {
            if (_lastStatus == value) return;
            _lastStatus = value ?? string.Empty;
            RaisePropertyChanged();
        }
    }

    public string SyncStatusColor
    {
        get => _syncStatusColor;
        set
        {
            if (_syncStatusColor == value) return;
            _syncStatusColor = value ?? "#6B7280";
            RaisePropertyChanged();
        }
    }
}

public class MobileConnectionViewModel : ViewModelBase
{
    private string _selectedFolderPath = string.Empty;
    private string _statusMessage = "Sélectionnez un dossier ou un téléphone pour synchroniser.";
    private string _statusBrush = "#6B7280"; // gray; green/red for folder sync result
    private bool _isSyncing;
    private bool _isGoogleConnected;
    private string _googleEmail = string.Empty;
    private string _googleStatusText = "Non connecté";
    private bool _isGoogleOperationInProgress;
    private bool _hasNetwork = true;
    private string _googleStatusBrush = "#6B7280"; // default gray

    public MobileConnectionViewModel()
    {
        PickFolderCommand = new RelayCommand(async () => await PickFolderAsync());
        SyncCommand = new RelayCommand(async () => await SyncAsync(), () => !string.IsNullOrWhiteSpace(SelectedFolderPath) && !_isSyncing);
        SyncToPhoneCommand = new RelayCommand<ConnectedPhoneItem>(async item => await SyncToPhoneAsync(item), item => item != null && !_isSyncing);
        RefreshPhonesCommand = new RelayCommand(RefreshPhones);
        GoogleLoginCommand = new RelayCommand(async () => await GoogleLoginAsync(), () => !_isGoogleOperationInProgress && HasNetwork);
        GoogleLogoutCommand = new RelayCommand(async () => await GoogleLogoutAsync(), () => !_isGoogleOperationInProgress);

        ConnectedPhones = new ObservableCollection<ConnectedPhoneItem>();
        RefreshPhones();
    }

    public RelayCommand PickFolderCommand { get; }
    public RelayCommand SyncCommand { get; }
    public RelayCommand<ConnectedPhoneItem> SyncToPhoneCommand { get; }
    public RelayCommand RefreshPhonesCommand { get; }
    public RelayCommand GoogleLoginCommand { get; }
    public RelayCommand GoogleLogoutCommand { get; }

    /// <summary>True if a Google account is connected (from local token).</summary>
    public bool IsGoogleConnected
    {
        get => _isGoogleConnected;
        set
        {
            if (_isGoogleConnected == value) return;
            _isGoogleConnected = value;
            RaisePropertyChanged();
            RaisePropertyChanged(nameof(IsGoogleDisconnected));
        }
    }

    /// <summary>True when not connected (for binding Log in section visibility).</summary>
    public bool IsGoogleDisconnected => !IsGoogleConnected;

    /// <summary>Connected user email, or empty when not connected.</summary>
    public string GoogleEmail
    {
        get => _googleEmail;
        set
        {
            if (_googleEmail == value) return;
            _googleEmail = value ?? string.Empty;
            RaisePropertyChanged();
        }
    }

    /// <summary>Status line under Google Drive (e.g. "Non connecté", "Connecté : user@...", "Connexion...").</summary>
    public string GoogleStatusText
    {
        get => _googleStatusText;
        set
        {
            if (_googleStatusText == value) return;
            _googleStatusText = value ?? string.Empty;
            RaisePropertyChanged();
        }
    }

    /// <summary>Brush (hex color) for the Google status text (gray by default, red on errors like no network).</summary>
    public string GoogleStatusBrush
    {
        get => _googleStatusBrush;
        set
        {
            if (_googleStatusBrush == value) return;
            _googleStatusBrush = value ?? "#6B7280";
            RaisePropertyChanged();
        }
    }

    /// <summary>True when a network connection is available.</summary>
    public bool HasNetwork
    {
        get => _hasNetwork;
        private set
        {
            if (_hasNetwork == value) return;
            _hasNetwork = value;
            RaisePropertyChanged();
            GoogleLoginCommand.RaiseCanExecuteChanged();
        }
    }

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

    /// <summary>Hex color for folder sync status: green success, red error, gray neutral.</summary>
    public string StatusBrush
    {
        get => _statusBrush;
        set
        {
            if (_statusBrush == value) return;
            _statusBrush = value ?? "#6B7280";
            RaisePropertyChanged();
        }
    }

    /// <summary>
    /// Loads Google connection state when the Mobile page is opened.
    /// First checks Internet availability via the InternetChecker service; if offline, shows a red warning
    /// and disables the Connect button. Otherwise, uses the cached ConnectedEmail in the service to decide
    /// the UI state.
    /// </summary>
    public async Task LoadGoogleStateAsync()
    {
        var internetChecker = ServiceProvider.InternetCheckerService;
        HasNetwork = await internetChecker.HasInternetAsync().ConfigureAwait(true);
        if (!HasNetwork)
        {
            IsGoogleConnected = false;
            GoogleEmail = string.Empty;
            GoogleStatusText = "Aucune connexion Internet détectée.";
            GoogleStatusBrush = "#DC2626"; // red-600
            return;
        }

        GoogleStatusBrush = "#6B7280"; // reset to default gray

        var svc = ServiceProvider.GoogleDriveConnectionService;

        if (!string.IsNullOrWhiteSpace(svc.ConnectedEmail))
        {
            IsGoogleConnected = true;
            GoogleEmail = svc.ConnectedEmail!;
            GoogleStatusText = $"Connecté : {GoogleEmail}";
        }
        else
        {
            IsGoogleConnected = false;
            GoogleEmail = string.Empty;
            GoogleStatusText = "Non connecté";
        }
    }

    private async Task GoogleLoginAsync()
    {
        var internetChecker = ServiceProvider.InternetCheckerService;
        HasNetwork = await internetChecker.HasInternetAsync().ConfigureAwait(true);
        if (!HasNetwork)
        {
            IsGoogleConnected = false;
            GoogleEmail = string.Empty;
            GoogleStatusText = "Aucune connexion Internet détectée.";
            GoogleStatusBrush = "#DC2626";
            return;
        }

        _isGoogleOperationInProgress = true;
        GoogleLoginCommand.RaiseCanExecuteChanged();
        GoogleLogoutCommand.RaiseCanExecuteChanged();
        GoogleStatusText = "Connexion à Google...";

        try
        {
            var svc = ServiceProvider.GoogleDriveConnectionService;
            await svc.ConnectAsync().ConfigureAwait(true);
            var email = await svc.GetConnectedEmailAsync().ConfigureAwait(true);
            IsGoogleConnected = true;
            GoogleEmail = email ?? string.Empty;
            GoogleStatusText = string.IsNullOrEmpty(GoogleEmail) ? "Connecté" : $"Connecté : {GoogleEmail}";
        }
        catch (Exception ex)
        {
            GoogleStatusText = "Erreur : " + ex.Message;
            IsGoogleConnected = false;
            GoogleEmail = string.Empty;
        }
        finally
        {
            _isGoogleOperationInProgress = false;
            GoogleLoginCommand.RaiseCanExecuteChanged();
            GoogleLogoutCommand.RaiseCanExecuteChanged();
        }
    }

    private async Task GoogleLogoutAsync()
    {
        _isGoogleOperationInProgress = true;
        GoogleLoginCommand.RaiseCanExecuteChanged();
        GoogleLogoutCommand.RaiseCanExecuteChanged();

        try
        {
            await ServiceProvider.GoogleDriveConnectionService.DisconnectAsync().ConfigureAwait(true);
            IsGoogleConnected = false;
            GoogleEmail = string.Empty;
            GoogleStatusText = "Non connecté";
        }
        finally
        {
            _isGoogleOperationInProgress = false;
            GoogleLoginCommand.RaiseCanExecuteChanged();
            GoogleLogoutCommand.RaiseCanExecuteChanged();
        }

        await Task.CompletedTask;
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
        {
            var uri = folders[0].Path;
            if (uri.IsAbsoluteUri && uri.IsFile)
                SelectedFolderPath = uri.LocalPath;
            else
            {
                StatusMessage = "Veuillez choisir un dossier sur ce PC (lecteur local).";
                SelectedFolderPath = string.Empty;
            }
        }
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
            StatusBrush = success ? "#16a34a" : "#DC2626"; // green / red, medium bold
        }
        catch (Exception ex)
        {
            StatusMessage = "Erreur : " + ex.Message;
            StatusBrush = "#DC2626";
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
        item.LastStatus = "Synchronisation en cours...";
        item.SyncStatusColor = "#6B7280";

        try
        {
            var service = ServiceProvider.ManualMobileDesktopSyncingService;
            var (success, message) = await service.SyncToMobileAsync(item.Index).ConfigureAwait(true);
            item.LastStatus = message;
            item.SyncStatusColor = success ? "#16a34a" : "#DC2626"; // green / red, medium bold
        }
        catch (Exception ex)
        {
            item.LastStatus = "Erreur : " + ex.Message;
            item.SyncStatusColor = "#DC2626";
        }
        finally
        {
            _isSyncing = false;
            SyncCommand.RaiseCanExecuteChanged();
            SyncToPhoneCommand.RaiseCanExecuteChanged();
        }
    }
}
