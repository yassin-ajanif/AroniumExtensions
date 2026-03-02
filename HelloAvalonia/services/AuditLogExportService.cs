using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using AroniumFactures.Data;
using Microsoft.EntityFrameworkCore;

namespace AroniumFactures.Services;

public class AuditLogExportService : IAuditLogExportService
{
    private readonly AppDbContext _context;

    /// <summary>
    /// Default directory: Desktop\AuditLogExports
    /// </summary>
    public static string DefaultExportDirectory =>
        Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
            "AuditLogExports"
        );

    /// <summary>
    /// Export file name: gzip-compressed CSV.
    /// </summary>
    public const string ExportFileName = "aronium-auditlog.csv.gz";

    private const string UploadStateFileName = "upload-state.json";

    public AuditLogExportService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<(string? Path, int MaxIdInFile)> ExportToCsvAsync()
    {
        Directory.CreateDirectory(DefaultExportDirectory);

        var filePath = Path.Combine(DefaultExportDirectory, ExportFileName);

        var currentDbMaxId = await _context.TableAuditLogs
            .MaxAsync(x => (int?)x.Id) ?? 0;

        var maxIdInExport = await GetMaxIdFromExportFileAsync(filePath);
        if (currentDbMaxId <= maxIdInExport)
            return (null, 0);

        var rows = await _context.TableAuditLogs
            .OrderBy(x => x.Id)
            .AsNoTracking()
            .ToListAsync();

        await using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
        await using (var gzip = new GZipStream(fileStream, CompressionLevel.SmallestSize))
        using (var writer = new StreamWriter(gzip, Encoding.UTF8))
        {
            await writer.WriteLineAsync("Id,TableName,Operation,SqlStatement,CreatedAt");
            foreach (var row in rows)
            {
                var line = string.Join(",",
                    EscapeCsv(row.Id.ToString()),
                    EscapeCsv(row.TableName ?? ""),
                    EscapeCsv(row.Operation ?? ""),
                    EscapeCsv(row.SqlStatement ?? ""),
                    EscapeCsv(row.CreatedAt ?? "")
                );
                await writer.WriteLineAsync(line);
            }
        }

        return (filePath, currentDbMaxId);
    }

    public async Task<int> GetMaxIdFromCsvFileAsync()
    {
        var filePath = Path.Combine(DefaultExportDirectory, ExportFileName);
        return await GetMaxIdFromExportFileAsync(filePath);
    }

    public async Task<int> GetLastUploadedIdAsync()
    {
        var statePath = Path.Combine(DefaultExportDirectory, UploadStateFileName);
        if (!File.Exists(statePath)) return 0;
        try
        {
            var json = await File.ReadAllTextAsync(statePath);
            var doc = JsonDocument.Parse(json);
            if (doc.RootElement.TryGetProperty("LastUploadedId", out var prop) && prop.TryGetInt32(out var id))
                return id;
        }
        catch
        {
            // corrupted or missing -> treat as 0
        }
        return 0;
    }

    public async Task SaveLastUploadedIdIntoJsonFile(int id)
    {
        Directory.CreateDirectory(DefaultExportDirectory);
        var statePath = Path.Combine(DefaultExportDirectory, UploadStateFileName);
        var json = JsonSerializer.Serialize(new { LastUploadedId = id });
        await File.WriteAllTextAsync(statePath, json);
    }

    /// <summary>
    /// Reads the existing .csv.gz export and returns the max Id from the first column of the decompressed CSV.
    /// Returns 0 if file missing or empty. Only considers lines that start with a digit (data rows).
    /// </summary>
    private static async Task<int> GetMaxIdFromExportFileAsync(string filePath)
    {
        if (!File.Exists(filePath)) return 0;
        var maxId = 0;
        try
        {
            await using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            using var gzip = new GZipStream(fileStream, CompressionMode.Decompress);
            using var reader = new StreamReader(gzip, Encoding.UTF8);
            var firstLine = true;
            string? line;
            while ((line = await reader.ReadLineAsync()) != null)
            {
                if (firstLine) { firstLine = false; continue; }
                if (line.Length > 0 && char.IsDigit(line[0]))
                {
                    var commaIdx = line.IndexOf(',');
                    var idStr = commaIdx > 0 ? line.Substring(0, commaIdx) : line;
                    if (int.TryParse(idStr.Trim(), out var id) && id > maxId)
                        maxId = id;
                }
            }
        }
        catch
        {
            return 0;
        }
        return maxId;
    }

    private static string EscapeCsv(string value)
    {
        if (value == null) return "\"\"";
        if (value.IndexOfAny(new[] { ',', '"', '\r', '\n' }) >= 0)
            return "\"" + value.Replace("\"", "\"\"") + "\"";
        return value;
    }
}
