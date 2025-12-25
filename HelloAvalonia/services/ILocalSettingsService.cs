using System.Threading.Tasks;
using HelloAvalonia.Models;

namespace HelloAvalonia.Services;

public interface ILocalSettingsService
{
    Task<AppSettings> LoadSettingsAsync();
    Task SaveSettingsAsync(AppSettings settings);
}

