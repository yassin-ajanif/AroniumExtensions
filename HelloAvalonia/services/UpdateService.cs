using System;
using System.Threading.Tasks;
using Velopack;
using Velopack.Sources;

namespace AroniumFactures.Services;

public class UpdateService : IUpdateService
{
    private UpdateManager? _updateManager;
    private bool _isUpdateAvailable;
    
    public bool IsUpdateAvailable => _isUpdateAvailable;

    private UpdateManager GetUpdateManager()
    {
        if (_updateManager != null)
            return _updateManager;

        _updateManager = new UpdateManager(new GithubSource("https://github.com/yassin-ajanif/AroniumExtensions", null, false));
        return _updateManager;
    }

    public async Task<UpdateInfo?> CheckForUpdatesAsync()
    {
        try
        {
            var manager = GetUpdateManager();
            var updateInfo = await manager.CheckForUpdatesAsync();
            _isUpdateAvailable = updateInfo != null;
            return updateInfo;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error checking for updates: {ex.Message}");
            _isUpdateAvailable = false;
            return null;
        }
    }

    public async Task DownloadUpdatesAsync(UpdateInfo updateInfo)
    {
        try
        {
            var manager = GetUpdateManager();
            await manager.DownloadUpdatesAsync(updateInfo);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error downloading updates: {ex.Message}");
            throw;
        }
    }

    public void ApplyUpdatesAndRestart(UpdateInfo updateInfo)
    {
        try
        {
            var manager = GetUpdateManager();
            manager.ApplyUpdatesAndRestart(updateInfo);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error applying updates: {ex.Message}");
            throw;
        }
    }
}

