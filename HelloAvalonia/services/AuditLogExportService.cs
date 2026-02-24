using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
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

    public AuditLogExportService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<string?> ExportToCsvAsync()
    {
        Directory.CreateDirectory(DefaultExportDirectory);

        var filePath = Path.Combine(DefaultExportDirectory, ExportFileName);

        var currentMaxId = await _context.TableAuditLogs
            .MaxAsync(x => (int?)x.Id) ?? 0;

        var maxIdInExport = await GetMaxIdFromExportFileAsync(filePath);
        if (currentMaxId <= maxIdInExport)
            return null;

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

        return filePath;
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
