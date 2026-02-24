using System.Threading.Tasks;

namespace AroniumFactures.Services;

public interface IGoogleDriveConnectionService
{
    /// <summary>
    /// Opens the browser for the user to sign in with their Google account (first run only).
    /// On subsequent runs, uses the saved token silently.
    /// </summary>
    Task ConnectAsync();

    /// <summary>
    /// Returns true if the user is already connected (saved token exists).
    /// </summary>
    Task<bool> IsConnectedAsync();

    /// <summary>
    /// Returns the connected Google account email, or null if not connected.
    /// </summary>
    Task<string?> GetConnectedEmailAsync();

    /// <summary>
    /// Disconnects the current user by deleting the saved token.
    /// </summary>
    Task DisconnectAsync();

    /// <summary>
    /// Uploads the CSV file to Google Drive in the "Aronium Audit" folder as aronium-auditlog.csv.
    /// Overwrites the existing file if present. Returns the Drive file Id, or null on failure.
    /// </summary>
    Task<string?> UploadCsvToDriveAsync(string localFilePath);
}
