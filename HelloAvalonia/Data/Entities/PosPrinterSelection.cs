using System;
using System.Collections.Generic;

namespace HelloAvalonia.Data.Entities;

public partial class PosPrinterSelection
{
    public int Id { get; set; }

    public string Key { get; set; } = null!;

    public string? PrinterName { get; set; }

    public int IsEnabled { get; set; }

    public virtual ICollection<PosPrinterSelectionSetting> PosPrinterSelectionSettings { get; set; } = new List<PosPrinterSelectionSetting>();
}
