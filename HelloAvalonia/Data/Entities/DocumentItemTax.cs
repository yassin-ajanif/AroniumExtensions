using System;
using System.Collections.Generic;

namespace AroniumFactures.Data.Entities;

public partial class DocumentItemTax
{
    public int DocumentItemId { get; set; }

    public int TaxId { get; set; }

    public double Amount { get; set; }

    public virtual DocumentItem DocumentItem { get; set; } = null!;

    public virtual Tax Tax { get; set; } = null!;
}
