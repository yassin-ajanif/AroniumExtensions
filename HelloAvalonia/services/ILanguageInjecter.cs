namespace AroniumFactures.Services;

/// <summary>
/// Injects French language files from the project translation folder into the installed Aronium directory.
/// Operation 1: Copy root fr to Aronium\Lang if missing.
/// Operation 2: Copy each module's Lang\fr into Aronium\Modules\{name}\Lang (only if Op1 copied).
/// </summary>
public interface ILanguageInjecter
{
    /// <summary>
    /// Runs Operation 1 (root fr), then Operation 2 (module fr files) only if root fr was copied.
    /// </summary>
    /// <returns>True if root fr was copied and module sync ran; false if root was skipped (fr already present).</returns>
    bool SyncTranslations();
}
