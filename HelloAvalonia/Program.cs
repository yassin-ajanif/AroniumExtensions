using Avalonia;
using System;
using System.IO;
using System.Linq;
using AroniumFactures.Services;
using System.Runtime.InteropServices;

namespace AroniumFactures;

class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args)
    {
        // Ensure native SkiaSharp binaries are discoverable when installed via MSI
        TryAddNativeSkiaPathToProcessPath();

        // Try to acquire mutex - if it already exists, another instance is running
        if (!SingleInstanceService.TryAcquireMutex())
        {
            // Another instance is already running - bring it to foreground
            SingleInstanceService.BringExistingInstanceToForeground();
            return;
        }

        try
        {
            BuildAvaloniaApp()
                .StartWithClassicDesktopLifetime(args);
        }
        
        finally
        {
            // Release the mutex when the application exits
            SingleInstanceService.ReleaseMutex();
        }
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();

    private static void TryAddNativeSkiaPathToProcessPath()
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return;
        }

        var archFolder = Environment.Is64BitProcess ? "win-x64" : "win-x86";
        var nativePath = Path.Combine(AppContext.BaseDirectory, "runtimes", archFolder, "native");

        if (!Directory.Exists(nativePath))
        {
            return;
        }

        var existingPath = Environment.GetEnvironmentVariable("PATH") ?? string.Empty;

        // Avoid duplicating the path entry if it is already present
        if (!existingPath.Split(Path.PathSeparator).Any(p => string.Equals(p, nativePath, StringComparison.OrdinalIgnoreCase)))
        {
            Environment.SetEnvironmentVariable("PATH", nativePath + Path.PathSeparator + existingPath);
        }

        // SkiaSharp also respects this hint
        Environment.SetEnvironmentVariable("SKIA_SHARP_NATIVE_LIBRARY_PATH", nativePath);
    }
}
