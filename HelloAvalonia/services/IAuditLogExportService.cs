using System.Threading.Tasks;

namespace AroniumFactures.Services;

public interface IAuditLogExportService
{
    /// <summary>
    /// Exports TableAuditLog to aronium-auditlog.csv.gz if there are new rows since last export.
    /// </summary>
    /// <returns>Path and max Id in the exported file, or (null, 0) if skipped (no new rows).</returns>
    Task<(string? Path, int MaxIdInFile)> ExportToCsvAsync();

    /// <summary>Max Id in the current CSV.gz file (0 if missing or empty).</summary>
    Task<int> GetMaxIdFromCsvFileAsync();

    /// <summary>Last audit log Id successfully uploaded to Google Drive (from upload-state.json).</summary>
    Task<int> GetLastUploadedIdAsync();

    /// <summary>Persist last uploaded Id after a successful upload.</summary>
    Task SaveLastUploadedIdIntoJsonFile(int id);
}
