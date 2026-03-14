using System;
using System.IO;

namespace AroniumFactures.Services;

/// <summary>
/// Injects French language files from the project translation folder into the installed Aronium directory.
/// </summary>
public class LanguageInjecter : ILanguageInjecter
{
    private const string LangFolderName = "Lang";
    private const string ModulesFolderName = "Modules";
    private static readonly string[] FrFileCandidates = { "fr.lang", "fr" };

    private readonly string _aroniumBasePath;
    private readonly string _translationRootPath;

    public LanguageInjecter(string? aroniumBasePath = null, string? translationRootPath = null)
    {
        _aroniumBasePath = aroniumBasePath ?? @"C:\Program Files\Aronium";
        _translationRootPath = translationRootPath ?? Path.Combine(AppContext.BaseDirectory, "translation");
    }

    /// <inheritdoc />
    public bool SyncTranslations()
    {
        bool didCopy = TryCopyRootFr();
        if (!didCopy)
            return false;
        CopyModuleTranslations();
        return true;
    }

    /// <summary>
    /// Operation 1: If Aronium\Lang already has an fr file, skip. Otherwise copy project root fr there.
    /// </summary>
    /// <returns>True if a copy was performed; false if skipped (fr already present).</returns>
    /// <exception cref="InvalidOperationException">Lang folder or source root fr missing.</exception>
    private bool TryCopyRootFr()
    {
        string langPath = Path.Combine(_aroniumBasePath, LangFolderName);
        if (!Directory.Exists(langPath))
            throw new InvalidOperationException($"Aronium Lang folder not found: {langPath}");

        string? existingFr = GetExistingFrFileName(langPath);
        if (existingFr != null)
            return false; // already has fr, skip

        string? sourceFr = GetExistingFrFileName(_translationRootPath);
        if (sourceFr == null)
            throw new InvalidOperationException($"Project translation root fr file not found in: {_translationRootPath}");

        string sourcePath = Path.Combine(_translationRootPath, sourceFr);
        string destPath = Path.Combine(langPath, sourceFr);
        File.Copy(sourcePath, destPath);
        return true;
    }

    /// <summary>
    /// Operation 2: For each module folder in translation root, copy its Lang\fr into Aronium\Modules\{name}\Lang.
    /// Does not create folders; throws if Modules subfolder or Lang subfolder is missing.
    /// </summary>
    private void CopyModuleTranslations()
    {
        if (!Directory.Exists(_translationRootPath))
            throw new InvalidOperationException($"Translation root not found: {_translationRootPath}");

        string modulesPath = Path.Combine(_aroniumBasePath, ModulesFolderName);
        if (!Directory.Exists(modulesPath))
            throw new InvalidOperationException($"Aronium Modules folder not found: {modulesPath}");

        foreach (string moduleDir in Directory.EnumerateDirectories(_translationRootPath))
        {
            string moduleName = Path.GetFileName(moduleDir);
            string sourceLangPath = Path.Combine(moduleDir, LangFolderName);
            if (!Directory.Exists(sourceLangPath))
                continue;

            string? sourceFr = GetExistingFrFileName(sourceLangPath);
            if (sourceFr == null)
                continue;

            string sourceFilePath = Path.Combine(sourceLangPath, sourceFr);
            if (!File.Exists(sourceFilePath))
                continue;

            string targetModulePath = Path.Combine(modulesPath, moduleName);
            if (!Directory.Exists(targetModulePath))
                throw new InvalidOperationException($"Module folder not found in Aronium: {targetModulePath}");

            string targetLangPath = Path.Combine(targetModulePath, LangFolderName);
            if (!Directory.Exists(targetLangPath))
                throw new InvalidOperationException($"Lang folder not found for module: {targetLangPath}");

            string targetFilePath = Path.Combine(targetLangPath, sourceFr);
            if (File.Exists(targetFilePath))
                continue; // already there, skip

            File.Copy(sourceFilePath, targetFilePath);
        }
    }

    /// <summary>
    /// Returns the first of fr.lang or fr that exists in the given directory, or null.
    /// </summary>
    private static string? GetExistingFrFileName(string directoryPath)
    {
        foreach (string name in FrFileCandidates)
        {
            string path = Path.Combine(directoryPath, name);
            if (File.Exists(path))
                return name;
        }
        return null;
    }
}
