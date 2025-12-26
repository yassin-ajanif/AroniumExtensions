using System.Threading.Tasks;

namespace AroniumFactures.Services;

public interface ISettingsService
{
    Task<string?> GetMainDatabasePathAsync();
    Task SaveMainDatabasePathAsync(string path);
    Task<bool> HasDatabasePathConfiguredAsync();
}

