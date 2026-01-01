using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;

namespace AroniumFactures.Services;

public static class SingleInstanceService
{
    private static Mutex? _mutex = null;
    private const string MutexName = "AroniumFactures_SingleInstance_Mutex";
    private const string WindowTitle = "Aronium Facture";
    private const string ProcessName = "HelloAvalonia";

    // Windows API declarations for bringing window to foreground
    [DllImport("user32.dll")]
    private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

    [DllImport("user32.dll")]
    private static extern bool SetForegroundWindow(IntPtr hWnd);

    [DllImport("user32.dll")]
    private static extern bool IsIconic(IntPtr hWnd);

    [DllImport("user32.dll")]
    private static extern IntPtr FindWindow(string? lpClassName, string? lpWindowName);

    private const int SW_RESTORE = 9;
    private const int SW_SHOW = 5;

    /// <summary>
    /// Attempts to acquire a mutex for single instance. Returns true if successful (first instance), false if another instance is running.
    /// </summary>
    public static bool TryAcquireMutex()
    {
        bool createdNew;
        _mutex = new Mutex(true, MutexName, out createdNew);
        return createdNew;
    }

    /// <summary>
    /// Releases the mutex and disposes it. Should be called when the application exits.
    /// </summary>
    public static void ReleaseMutex()
    {
        _mutex?.ReleaseMutex();
        _mutex?.Dispose();
        _mutex = null;
    }

    /// <summary>
    /// Brings the existing application instance window to the foreground.
    /// </summary>
    public static void BringExistingInstanceToForeground()
    {
        try
        {
            // Find the window by title
            IntPtr hWnd = FindWindow(null, WindowTitle);
            
            if (hWnd != IntPtr.Zero)
            {
                RestoreAndActivateWindow(hWnd);
                return;
            }

            // If window not found by title, try to find by process name
            Process[] processes = Process.GetProcessesByName(ProcessName);
            if (processes.Length > 0)
            {
                // Wait a bit for the window to be created
                Thread.Sleep(500);
                hWnd = FindWindow(null, WindowTitle);
                if (hWnd != IntPtr.Zero)
                {
                    RestoreAndActivateWindow(hWnd);
                }
            }
        }
        catch
        {
            // If bringing to foreground fails, just exit silently
        }
    }

    private static void RestoreAndActivateWindow(IntPtr hWnd)
    {
        // Check if window is minimized
        if (IsIconic(hWnd))
        {
            ShowWindow(hWnd, SW_RESTORE);
        }
        else
        {
            ShowWindow(hWnd, SW_SHOW);
        }
        
        // Bring window to foreground
        SetForegroundWindow(hWnd);
    }
}











