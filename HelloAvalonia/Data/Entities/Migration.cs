using System;
using System.Collections.Generic;

namespace HelloAvalonia.Data.Entities;

public partial class Migration
{
    public int Id { get; set; }

    public decimal Version { get; set; }

    public string? Description { get; set; }

    public string FileName { get; set; } = null!;

    public string? Module { get; set; }

    public DateTime? Date { get; set; }
}
