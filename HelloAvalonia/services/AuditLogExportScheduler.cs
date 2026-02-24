using System;
using System.Threading;
using System.Threading.Tasks;

namespace AroniumFactures.Services;

/// <summary>
/// Runs the audit log export at a configurable interval. Call Start() once after the app is initialized.
/// When an upload is in progress, the next cycle skips both export and upload to avoid overlapping uploads.
/// </summary>
public class AuditLogExportScheduler
{
    private readonly IAuditLogExportService _exportService;
    private readonly IGoogleDriveConnectionService _googleDriveService;
    private readonly int _intervalMinutes;
    private readonly int _intervalSeconds;
    private CancellationTokenSource? _cts;

    private bool _uploadInProgress;

    /// <summary>
    /// Creates a scheduler with the given interval.
    /// </summary>
    /// <param name="exportService">The export service to call.</param>
    /// <param name="googleDriveService">Google Drive service for uploading the audit file.</param>
    /// <param name="intervalMinutes">Interval in minutes (default 5).</param>
    /// <param name="intervalSeconds">Additional interval in seconds (default 0).</param>
    public AuditLogExportScheduler(IAuditLogExportService exportService, IGoogleDriveConnectionService googleDriveService, int intervalMinutes = 5, int intervalSeconds = 0)
    {
        _exportService = exportService;
        _googleDriveService = googleDriveService;
        _intervalMinutes = Math.Max(0, intervalMinutes);
        _intervalSeconds = Math.Max(0, intervalSeconds);
        if (_intervalMinutes == 0 && _intervalSeconds == 0)
            _intervalSeconds = 60;
    }

    /// <summary>
    /// Starts the scheduler. Exports immediately, then every (intervalMinutes + intervalSeconds). Runs in the background.
    /// </summary>
    public void Start() => Start(_intervalMinutes, _intervalSeconds);

    /// <summary>
    /// Starts the scheduler with an explicit interval. Exports immediately, then every (minutes + seconds). Runs in the background.
    /// </summary>
    /// <param name="minutes">Interval in minutes.</param>
    /// <param name="seconds">Additional interval in seconds.</param>
    public void Start(int minutes, int seconds = 0)
    {
        var interval = TimeSpan.FromMinutes(Math.Max(0, minutes)) + TimeSpan.FromSeconds(Math.Max(0, seconds));
        if (interval <= TimeSpan.Zero)
            interval = TimeSpan.FromMinutes(1);

        _cts?.Cancel();
        _cts = new CancellationTokenSource();
        var token = _cts.Token;

        _ = Task.Run(() => RunLoopAsync(interval, token), token);
    }

    /// <summary>
    /// Stops the scheduler (e.g. on app shutdown).
    /// </summary>
    public void Stop() => _cts?.Cancel();

    private async Task RunLoopAsync(TimeSpan interval, CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            try
            {
                await RunOneCycleAsync(token);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[AuditLogExport] Error: {ex.Message}");
            }

            await DelayAsync(interval, token);
        }
    }

    private async Task RunOneCycleAsync(CancellationToken token)
    {
        if (IsUploadInProgress())
        {
            LogSkippedUploadInProgress();
            return;
        }

        var path = await ExportAsync();
        if (path == null)
        {
            LogSkippedNoNewRows();
            return;
        }

        LogExported(path);
        if (!TryBeginUpload())
        {
            LogSkippedUploadAlreadyInProgress();
            return;
        }

        try
        {
            await UploadAndLogAsync(path);
        }
        finally
        {
            EndUpload();
        }
    }

    private bool IsUploadInProgress() => _uploadInProgress;

    private bool TryBeginUpload()
    {
        if (_uploadInProgress)
            return false;
        _uploadInProgress = true;
        return true;
    }

    private void EndUpload() => _uploadInProgress = false;

    private async Task<string?> ExportAsync()
    {
        return await _exportService.ExportToCsvAsync();
    }

    private async Task UploadAndLogAsync(string path)
    {
        try
        {
            var driveFileId = await _googleDriveService.UploadCsvToDriveAsync(path);
            if (driveFileId != null)
                Console.WriteLine($"[AuditLogExport] Uploaded to Google Drive (file Id: {driveFileId})");
            else
                Console.WriteLine("[AuditLogExport] Google Drive upload skipped or failed (not connected or error)");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[AuditLogExport] Google Drive upload error: {ex.Message}");
        }
    }

    private static void LogSkippedUploadInProgress()
    {
        Console.WriteLine("[AuditLogExport] Skipped (upload still in progress)");
    }

    private static void LogSkippedNoNewRows()
    {
        Console.WriteLine("[AuditLogExport] Skipped (no new audit rows)");
    }

    private static void LogSkippedUploadAlreadyInProgress()
    {
        Console.WriteLine("[AuditLogExport] Skipped upload (upload already in progress)");
    }

    private static void LogExported(string path)
    {
        Console.WriteLine($"[AuditLogExport] Exported to {path}");
    }

    private static async Task DelayAsync(TimeSpan interval, CancellationToken token)
    {
        try
        {
            await Task.Delay(interval, token);
        }
        catch (OperationCanceledException)
        {
            // expected when stopping
        }
    }
}
