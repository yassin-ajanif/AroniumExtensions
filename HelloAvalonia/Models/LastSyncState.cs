namespace AroniumFactures.Models;

/// <summary>
/// JSON model for the last_audit_import file in aroniumreport folder (manual mobile sync).
/// </summary>
public class LastSyncState
{
    public int LastInjectedCsvRowId { get; set; }
}
