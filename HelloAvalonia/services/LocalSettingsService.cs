using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using HelloAvalonia.Models;

namespace HelloAvalonia.Services;

public class LocalSettingsService : ILocalSettingsService
{
    private static readonly string SettingsFolder = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "HelloAvalonia"
    );
    
    private static readonly string SettingsFile = Path.Combine(SettingsFolder, "appsettings.json");
    
    public LocalSettingsService()
    {
        Directory.CreateDirectory(SettingsFolder);
    }
    
    public async Task<AppSettings> LoadSettingsAsync()
    {
        try
        {
            if (File.Exists(SettingsFile))
            {
                var json = await File.ReadAllTextAsync(SettingsFile);
                return JsonSerializer.Deserialize<AppSettings>(json) ?? new AppSettings();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading settings: {ex.Message}");
        }
        
        return new AppSettings();
    }
    
    public async Task SaveSettingsAsync(AppSettings settings)
    {
        try
        {
            settings.LastModified = DateTime.Now;
            var json = JsonSerializer.Serialize(settings, new JsonSerializerOptions
            {
                WriteIndented = true
            });
            await File.WriteAllTextAsync(SettingsFile, json);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving settings: {ex.Message}");
        }
    }
}

