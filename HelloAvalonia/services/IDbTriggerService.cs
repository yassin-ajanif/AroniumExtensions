namespace AroniumFactures.Services;

/// <summary>
/// Ensures that the audit infrastructure (TableAuditLog table and audit triggers)
/// exists in the target SQLite database.
/// </summary>
public interface IDbTriggerService
{
    /// <summary>
    /// Ensures that TableAuditLog and all required audit triggers exist.
    /// If the table already exists, no action is taken.
    /// If it does not exist, the table and all triggers are created
    /// inside a single transaction (all or nothing).
    /// </summary>
    void EnsureAuditInfrastructure();
}

