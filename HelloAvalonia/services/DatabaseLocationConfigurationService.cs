using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace AroniumFactures.Services;

public static class DatabaseLocationConfigurationService
{
    private const string DefaultMainDbPath = @"C:\ProgramData\Aronium\SimplePos\pos.db";
    
    /// <summary>
    /// Gets the base path for aronium Extensions folder in Local AppData
    /// </summary>
    public static string GetAroniumExtensionsPath()
    {
        string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        return Path.Combine(localAppData, "aronium Extensions");
    }
    
    /// <summary>
    /// Gets the database folder path within aronium Extensions
    /// </summary>
    public static string GetDatabaseFolderPath()
    {
        return Path.Combine(GetAroniumExtensionsPath(), "database");
    }
    
    /// <summary>
    /// Gets the full path to location.json file
    /// </summary>
    public static string GetLocationJsonPath()
    {
        return Path.Combine(GetDatabaseFolderPath(), "location.json");
    }
    
    /// <summary>
    /// Ensures the aronium Extensions folder structure exists and creates location.json if needed
    /// This should be called on app startup
    /// </summary>
    public static void EnsureFolderStructure()
    {
        try
        {
            string databaseFolder = GetDatabaseFolderPath();
            string locationFile = GetLocationJsonPath();
            
            // Create database folder if it doesn't exist
            if (!Directory.Exists(databaseFolder))
            {
                Directory.CreateDirectory(databaseFolder);
                Console.WriteLine($"Created aronium Extensions folder: {databaseFolder}");
            }
            
            // Create location.json with default values if it doesn't exist
            if (!File.Exists(locationFile))
            {
                var defaultSettings = new LocationSettings
                {
                    MainDatabasePath = DefaultMainDbPath
                };
                
                var options = new JsonSerializerOptions { WriteIndented = true };
                string json = JsonSerializer.Serialize(defaultSettings, options);
                File.WriteAllText(locationFile, json);
                
                Console.WriteLine($"Created default location.json: {locationFile}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error ensuring aronium Extensions folder structure: {ex.Message}");
            throw;
        }
    }
    
    /// <summary>
    /// Gets the main database path from location.json
    /// </summary>
    public static string GetMainDatabasePath()
    {
        try
        {
            string locationFile = GetLocationJsonPath();
            
            if (!File.Exists(locationFile))
            {
                // If file doesn't exist, ensure folder structure and return default
                EnsureFolderStructure();
                return DefaultMainDbPath;
            }
            
            using FileStream fileStream = File.OpenRead(locationFile);
            using JsonDocument doc = JsonDocument.Parse(fileStream);
            
            if (doc.RootElement.TryGetProperty("MainDatabasePath", out JsonElement pathElement))
            {
                string? configuredPath = pathElement.GetString();
                if (!string.IsNullOrWhiteSpace(configuredPath))
                {
                    return configuredPath;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error reading location.json: {ex.Message}");
        }
        
        return DefaultMainDbPath;
    }
    
    /// <summary>
    /// Saves the main database path to location.json
    /// </summary>
    public static async Task SaveMainDatabasePathAsync(string databasePath)
    {
        try
        {
            // Ensure folder structure exists
            EnsureFolderStructure();
            
            string locationFile = GetLocationJsonPath();
            var settings = new LocationSettings
            {
                MainDatabasePath = databasePath
            };
            
            var options = new JsonSerializerOptions { WriteIndented = true };
            string json = JsonSerializer.Serialize(settings, options);
            await File.WriteAllTextAsync(locationFile, json);
            
            Console.WriteLine($"Saved database path to: {locationFile}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving location.json: {ex.Message}");
            throw;
        }
    }
    
    /// <summary>
    /// Loads the current database path from location.json
    /// </summary>
    public static async Task<string> LoadMainDatabasePathAsync()
    {
        try
        {
            string locationFile = GetLocationJsonPath();
            
            if (!File.Exists(locationFile))
            {
                // If file doesn't exist, ensure folder structure and return default
                EnsureFolderStructure();
                return DefaultMainDbPath;
            }
            
            string json = await File.ReadAllTextAsync(locationFile);
            var settings = JsonSerializer.Deserialize<LocationSettings>(json);
            
            if (settings != null && !string.IsNullOrWhiteSpace(settings.MainDatabasePath))
            {
                return settings.MainDatabasePath;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading location.json: {ex.Message}");
        }
        
        return DefaultMainDbPath;
    }
    
    /// <summary>
    /// Internal class for deserializing location.json
    /// </summary>
    private class LocationSettings
    {
        public string? MainDatabasePath { get; set; }
    }
}











