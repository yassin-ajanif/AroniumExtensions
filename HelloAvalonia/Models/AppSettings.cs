using System;

namespace AroniumFactures.Models;

public class AppSettings
{
    public string? LogoPath { get; set; }
    public string? CompanyName { get; set; }
    public string? IceNumber { get; set; }
    public string? CompanyInfo { get; set; }

    /// <summary>Path to the main companion app exe (e.g. Aronium.Pos.exe). Used to start and watch.</summary>
    public string? CompanionAppExePath { get; set; }
    
    public DateTime LastModified { get; set; } = DateTime.Now;
}

