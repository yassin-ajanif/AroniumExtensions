using System;
using System.Collections.Generic;

namespace AroniumFactures.Data.Entities;

public partial class Tax
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public decimal Rate { get; set; }

    public string? Code { get; set; }

    public int IsFixed { get; set; }

    public int IsTaxOnTotal { get; set; }

    public int IsEnabled { get; set; }

    public virtual ICollection<DocumentItemTax> DocumentItemTaxes { get; set; } = new List<DocumentItemTax>();

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
