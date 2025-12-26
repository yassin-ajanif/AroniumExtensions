using System.Threading.Tasks;
using AroniumFactures.Models;

namespace AroniumFactures.Services;

public interface ILocalSettingsService
{
    Task<AppSettings> LoadSettingsAsync();
    Task SaveSettingsAsync(AppSettings settings);
}

