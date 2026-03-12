using System.Collections.Generic;
using System.Threading.Tasks;

namespace AroniumFactures.Services;

public interface IManualMobileDesktopSyncingService
{
    /// <summary>
    /// Returns the list of connected MTP devices (index, display name) for the user to choose from.
    /// </summary>
    IReadOnlyList<(int Index, string FriendlyName)> GetConnectedDevices();

    /// <summary>
    /// Creates aroniumreport in the given folder, copies the current database there,
    /// and writes last_audit_import JSON with the current max TableAuditLog row id.
    /// </summary>
    Task<(bool Success, string Message)> SyncToFolderAsync(string rootFolderPath);

    /// <summary>
    /// Syncs directly to the connected MTP device at the given index (from GetConnectedDevices).
    /// </summary>
    Task<(bool Success, string Message)> SyncToMobileAsync(int deviceIndex);
}
