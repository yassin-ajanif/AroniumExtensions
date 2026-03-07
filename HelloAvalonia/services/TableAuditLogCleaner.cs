using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Data.Sqlite;

namespace AroniumFactures.Services;

public class TableAuditLogCleaner : ITableAuditLogCleaner
{
    private const string IndexFileName = "last_audit_import.json";
    private const string IndexPropertyName = "lastInjectedCsvRowId";

    private readonly IGoogleDriveConnectionService _googleDriveService;
    private readonly string _databasePath;

    public TableAuditLogCleaner(IGoogleDriveConnectionService googleDriveService, string databasePath)
    {
        _googleDriveService = googleDriveService ?? throw new ArgumentNullException(nameof(googleDriveService));
        _databasePath = databasePath ?? throw new ArgumentNullException(nameof(databasePath));
    }

    public async Task CleanAsync()
    {
        try
        {
            var json = await _googleDriveService.DownloadAuditFolderFileContentAsync(IndexFileName);
            if (string.IsNullOrWhiteSpace(json))
            {
                Console.WriteLine("[TableAuditLogCleaner] No index file or download failed; skip wipe.");
                return;
            }

            var lastInjectedId = ParseLastInjectedCsvRowId(json);
            if (lastInjectedId <= 0)
            {
                Console.WriteLine("[TableAuditLogCleaner] Invalid or missing lastInjectedCsvRowId; skip wipe.");
                return;
            }

            if (!File.Exists(_databasePath))
            {
                Console.WriteLine($"[TableAuditLogCleaner] Database not found: {_databasePath}");
                return;
            }

            var deleted = await DeleteOlderRowsAsync(lastInjectedId);
            if (deleted >= 0)
                Console.WriteLine($"[TableAuditLogCleaner] Deleted {deleted} rows with Id < {lastInjectedId}.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[TableAuditLogCleaner] Error: {ex.Message}");
        }
    }

    private static int ParseLastInjectedCsvRowId(string json)
    {
        try
        {
            var doc = JsonDocument.Parse(json);
            if (doc.RootElement.TryGetProperty(IndexPropertyName, out var prop) && prop.TryGetInt32(out var id))
                return id;
        }
        catch
        {
            // ignore parse errors
        }

        return 0;
    }

    private async Task<int> DeleteOlderRowsAsync(int lastInjectedId)
    {
        var connectionString = $"Data Source={_databasePath};Mode=ReadWrite;Default Timeout=10;";
        await using var connection = new SqliteConnection(connectionString);
        await connection.OpenAsync();

        await using var cmd = connection.CreateCommand();
        cmd.CommandText = "DELETE FROM TableAuditLog WHERE Id < @id";
        cmd.Parameters.AddWithValue("@id", lastInjectedId);
        var deleted = await cmd.ExecuteNonQueryAsync();
        return deleted;
    }
}
