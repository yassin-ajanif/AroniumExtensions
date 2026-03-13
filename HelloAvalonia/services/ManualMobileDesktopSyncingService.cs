using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Versioning;
using System.Text.Json;
using System.Threading.Tasks;
using AroniumFactures.Data;
using AroniumFactures.Models;
using MediaDevices;
using Microsoft.EntityFrameworkCore;

namespace AroniumFactures.Services;

public class ManualMobileDesktopSyncingService : IManualMobileDesktopSyncingService
{
    private const string ReportFolderName = "aroniumreport";
    private const string LastAuditImportFileName = "last_audit_import.json";

    /// <summary>
    /// Deletes the folder at the given path if it exists (local filesystem).
    /// Call this before creating the folder and copying content so the destination is fresh.
    /// </summary>
    private static void DeleteFolderIfExists(string localPath)
    {
        if (string.IsNullOrWhiteSpace(localPath)) return;
        if (Directory.Exists(localPath))
            Directory.Delete(localPath, recursive: true);
    }

    /// <summary>
    /// Deletes the folder on the MTP device if it exists.
    /// Call this before creating the folder and uploading content (MTP does not overwrite files).
    /// </summary>
   /* [SupportedOSPlatform("windows")]
    private static void DeleteFolderIfExists(MediaDevice device, string remotePath)
    {
        if (device == null || string.IsNullOrWhiteSpace(remotePath)) return;
        if (!device.DirectoryExists(remotePath))
            return;

        // MediaDevice.DeleteDirectory only works on empty directories, so remove children first.
        void DeleteRecursive(string path)
        {
            foreach (var file in device.GetFiles(path))
                device.DeleteFile(file);

            foreach (var dir in device.GetDirectories(path))
                DeleteRecursive(dir);

            device.DeleteDirectory(path);
        }

        DeleteRecursive(remotePath);
    }
*/


[SupportedOSPlatform("windows")]
private static void DeleteFolderIfExists(MediaDevice device, string remotePath)
{
    if (device == null || string.IsNullOrWhiteSpace(remotePath)) return;
    
    try
    {
        if (device.DirectoryExists(remotePath))
        {
            // The second parameter 'true' tells the library to delete 
            // all files and subfolders automatically.
            device.DeleteDirectory(remotePath, true);
        }
    }
    catch (Exception ex)
    {
        // Log or handle the error if the folder is locked by the phone's OS
        System.Diagnostics.Debug.WriteLine($"MTP Delete Error: {ex.Message}");
    }
}
   
   
   
    [SupportedOSPlatform("windows")]
    public IReadOnlyList<(int Index, string FriendlyName)> GetConnectedDevices()
    {
        try
        {
            var devices = MediaDevice.GetDevices().ToList();
            var list = new List<(int, string)>();
            for (int i = 0; i < devices.Count; i++)
                list.Add((i, devices[i].FriendlyName ?? ("Appareil " + (i + 1))));
            return list;
        }
        catch
        {
            return Array.Empty<(int, string)>();
        }
    }

   [SupportedOSPlatform("windows")]
public async Task<(bool Success, string Message)> SyncToMobileAsync(int deviceIndex)
{
    MediaDevice? phone = null;

    try
    {
        var devices = MediaDevice.GetDevices().ToList();
        if (deviceIndex < 0 || deviceIndex >= devices.Count)
            return (false, "Appareil non trouvé. Actualisez la liste.");

        phone = devices[deviceIndex];

        await Task.Delay(1500).ConfigureAwait(false); // give MTP time after plug-in

        if (!phone.IsConnected)
            phone.Connect();

        var drives = phone.GetDrives().ToList();
        var drive = drives.FirstOrDefault();
        if (drive == null)
            return (false, "Impossible d'accéder au stockage du téléphone.");

        string remoteReportPath = Path.Combine(drive.RootDirectory.FullName, ReportFolderName);
        DeleteFolderIfExists(phone, remoteReportPath);
        phone.CreateDirectory(remoteReportPath);

        var dbPath = DatabaseLocationConfigurationService.GetMainDatabasePath();
        if (!File.Exists(dbPath))
            return (false, "Base de données introuvable.");

        string tempDbPath = Path.Combine(Path.GetTempPath(), "aronium_sync_" + Path.GetFileName(dbPath));
        try
        {
            await Task.Run(() => File.Copy(dbPath, tempDbPath, overwrite: true)).ConfigureAwait(false);
            string destDbPath = Path.Combine(remoteReportPath, Path.GetFileName(dbPath));
            await Task.Run(() => phone.UploadFile(tempDbPath, destDbPath)).ConfigureAwait(false);
        }
        finally
        {
            try { if (File.Exists(tempDbPath)) File.Delete(tempDbPath); } catch { }
        }

        int lastRowId = 0;
        try
        {
            var db = ServiceProvider.DbContext;
            if (db.TableAuditLogs.Any())
                lastRowId = await db.TableAuditLogs.MaxAsync(x => x.Id).ConfigureAwait(false);
        }
        catch
        {
        }

        var state = new LastSyncState { LastInjectedCsvRowId = lastRowId };
        var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        var json = JsonSerializer.Serialize(state, options);

        string tempJsonPath = Path.Combine(Path.GetTempPath(), LastAuditImportFileName);
        await File.WriteAllTextAsync(tempJsonPath, json).ConfigureAwait(false);

        try
        {
            string destJsonPath = Path.Combine(remoteReportPath, LastAuditImportFileName);
            await Task.Run(() => phone.UploadFile(tempJsonPath, destJsonPath)).ConfigureAwait(false);
        }
        finally
        {
            try { File.Delete(tempJsonPath); } catch { }
        }

        try
        {
            if (lastRowId > 0)
                await TableAuditLogCleaner.DeleteOlderRowsAsync(dbPath, lastRowId).ConfigureAwait(false);
        }
        catch
        {
        }

        return (true, "Synchronisé vers le téléphone : " + (phone.FriendlyName ?? "appareil") + ".");
    }
    catch (Exception ex)
    {
        return (false, "MTP : " + ex.Message);
    }
    finally
    {
        try
        {
            if (phone != null && phone.IsConnected)
                phone.Disconnect();
        }
        catch
        {
        }
    }
}
       public async Task<(bool Success, string Message)> SyncToFolderAsync(string rootFolderPath)
    {
        if (string.IsNullOrWhiteSpace(rootFolderPath))
            return (false, "Dossier non sélectionné.");

        try
        {
            var dbPath = DatabaseLocationConfigurationService.GetMainDatabasePath();
            if (!File.Exists(dbPath))
                return (false, "Base de données introuvable.");

            var reportPath = Path.Combine(rootFolderPath, ReportFolderName);
            DeleteFolderIfExists(reportPath);
            Directory.CreateDirectory(reportPath);

            var dbFileName = Path.GetFileName(dbPath);
            var destDbPath = Path.Combine(reportPath, dbFileName);
            await Task.Run(() => File.Copy(dbPath, destDbPath, overwrite: true)).ConfigureAwait(false);

            int lastRowId = 0;
            try
            {
                var db = ServiceProvider.DbContext;
                if (db.TableAuditLogs.Any())
                    lastRowId = await db.TableAuditLogs.MaxAsync(x => x.Id).ConfigureAwait(false);
            }
            catch
            {
                // Keep 0 if we can't read (e.g. table missing)
            }

            var state = new LastSyncState { LastInjectedCsvRowId = lastRowId };
            var jsonPath = Path.Combine(reportPath, LastAuditImportFileName);
            var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
            var json = JsonSerializer.Serialize(state, options);
            await File.WriteAllTextAsync(jsonPath, json).ConfigureAwait(false);

            // After successful export, prune older audit logs locally.
            try
            {
                if (lastRowId > 0)
                    await TableAuditLogCleaner.DeleteOlderRowsAsync(dbPath, lastRowId).ConfigureAwait(false);
            }
            catch
            {
                // ignore cleanup failures to avoid blocking sync
            }

            return (true, "Synchronisation terminée. Dossier : " + reportPath);
        }
        catch (Exception ex)
        {
            return (false, "Erreur : " + ex.Message);
        }
    }
}
