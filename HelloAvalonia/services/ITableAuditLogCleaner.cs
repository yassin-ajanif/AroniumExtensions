using System.Threading.Tasks;

namespace AroniumFactures.Services;

/// <summary>
/// Runs at app launch: downloads last_audit_import.json from Google Drive and deletes older rows from TableAuditLog
/// to keep the table small and reduce upload bandwidth.
/// </summary>
public interface ITableAuditLogCleaner
{
    Task CleanAsync();
}
