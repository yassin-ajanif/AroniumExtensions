using System.Threading.Tasks;

namespace HelloAvalonia.Services;

public interface ISettingsService
{
    Task<string?> GetMainDatabasePathAsync();
    Task SaveMainDatabasePathAsync(string path);
    Task<bool> HasDatabasePathConfiguredAsync();
}

