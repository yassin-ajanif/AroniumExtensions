using System;

namespace HelloAvalonia.Models;

public class AppSettings
{
    public string? LogoPath { get; set; }
    public string? CompanyName { get; set; }
    public string? IceNumber { get; set; }
    public string? CompanyInfo { get; set; }
    
    public DateTime LastModified { get; set; } = DateTime.Now;
}

