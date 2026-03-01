using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;

namespace AroniumFactures.Services;

public class CompanionAppWatcherService : ICompanionAppWatcherService
{
    private const int CheckIntervalMs = 2000;
    private volatile bool _stopRequested;
    private volatile int _runId;
    private int _watchedProcessId;

    public void Start(int watchedProcessId)
    {
        _watchedProcessId = watchedProcessId;
        _stopRequested = false;
        var runId = ++_runId;
        _ = Task.Run(() => RunLoopAsync(runId));
    }

    public void Stop() => _stopRequested = true;

    private async Task RunLoopAsync(int runId)
    {
        while (!_stopRequested && _runId == runId)
        {
            try
            {
                if (HasProcessExited(_watchedProcessId))
                {
                    Avalonia.Threading.Dispatcher.UIThread.Post(() =>
                    {
                        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
                            desktop.Shutdown();
                    });
                    return;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[CompanionAppWatcher] Error: {ex.Message}");
            }

            await Task.Delay(CheckIntervalMs);
        }
    }

    private static bool HasProcessExited(int processId)
    {
        try
        {
            using var p = Process.GetProcessById(processId);
            return p.HasExited;
        }
        catch (ArgumentException)
        {
            return true;
        }
    }
}
