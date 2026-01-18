using System.Threading.Tasks;
using Velopack;

namespace AroniumFactures.Services;

public interface IUpdateService
{
    Task<UpdateInfo?> CheckForUpdatesAsync();
    Task DownloadUpdatesAsync(UpdateInfo updateInfo);
    void ApplyUpdatesAndRestart(UpdateInfo updateInfo);
    bool IsUpdateAvailable { get; }
}







