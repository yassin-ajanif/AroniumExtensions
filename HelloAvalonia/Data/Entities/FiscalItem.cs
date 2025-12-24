using System;
using System.Collections.Generic;

namespace HelloAvalonia.Data.Entities;

public partial class FiscalItem
{
    public int Plu { get; set; }

    public string Name { get; set; } = null!;

    public string Vat { get; set; } = null!;
}
