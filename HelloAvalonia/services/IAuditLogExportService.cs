using System.Threading.Tasks;

namespace AroniumFactures.Services;

public interface IAuditLogExportService
{
    /// <summary>
    /// Exports TableAuditLog to aronium-auditlog.csv.gz (gzip-compressed CSV) if there are new rows since last export.
    /// Skips export (returns null) when max Id in DB equals max Id in existing export file.
    /// </summary>
    /// <returns>Full path of the .csv.gz file, or null if skipped (no new rows).</returns>
    Task<string?> ExportToCsvAsync();
}
