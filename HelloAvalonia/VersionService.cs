using System;
using System.IO;
using System.Linq;
using System.Reflection;

namespace AroniumFactures;

public static class VersionService
{
    private static string? _cachedVersion;

    public static string GetInstalledVersion()
    {
        if (_cachedVersion != null)
            return _cachedVersion;

        try
        {
            // Method 1: Check if we're running from a versioned directory structure
            // Some installers use: [InstallDir]/[version]/[exe]
            var assembly = Assembly.GetExecutingAssembly();
            var appDir = Path.GetDirectoryName(assembly.Location);
            
            if (appDir != null)
            {
                var currentDir = new DirectoryInfo(appDir);
                var parentDir = currentDir.Parent;

                // Check if parent directory is a version number
                if (parentDir != null && System.Version.TryParse(parentDir.Name, out var version))
                {
                    _cachedVersion = version.ToString(3);
                    return _cachedVersion;
                }
            }

            // Method 2: Check for version in common application data directories
            var appName = Assembly.GetExecutingAssembly().GetName().Name ?? "AroniumFactures";
            var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var programData = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            
            // Check common installation locations
            var possiblePaths = new[]
            {
                Path.Combine(localAppData, appName, "packages"),
                Path.Combine(localAppData, appName, "versions"),
                Path.Combine(programData, appName, "packages"),
                Path.Combine(programData, appName, "versions")
            };

            foreach (var versionDir in possiblePaths)
            {
                if (Directory.Exists(versionDir))
                {
                    var version = GetLatestVersionFromDirectory(versionDir);
                    if (version != null)
                    {
                        _cachedVersion = version;
                        return _cachedVersion;
                    }
                }
            }

            // Method 3: Read from AssemblyInformationalVersion (from <Version> property in .csproj)
            var informationalVersion = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;
            
            if (!string.IsNullOrEmpty(informationalVersion))
            {
                // Remove any build metadata (e.g., "1.0.1+abc123" -> "1.0.1")
                var version = informationalVersion.Split('+')[0].Split('-')[0];
                _cachedVersion = version;
                return _cachedVersion;
            }

            // Method 4: Fallback to assembly version if informational version is not available
            var assemblyVersion = assembly.GetName().Version?.ToString(3);
            
            if (assemblyVersion != null)
            {
                _cachedVersion = assemblyVersion;
                return _cachedVersion;
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error reading version: {ex.Message}");
        }

        _cachedVersion = "no version detected";
        return _cachedVersion;
    }

    private static string? GetLatestVersionFromDirectory(string versionDir)
    {
        try
        {
            var versionDirs = Directory.GetDirectories(versionDir)
                .Select(dir => Path.GetFileName(dir))
                .Where(name => System.Version.TryParse(name, out _))
                .OrderByDescending(name => new System.Version(name))
                .ToList();

            return versionDirs.FirstOrDefault();
        }
        catch
        {
            return null;
        }
    }
}

