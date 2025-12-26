using System;
using System.Collections.Generic;

namespace AroniumFactures.Data.Entities;

public partial class StockControl
{
    public int Id { get; set; }

    public int ProductId { get; set; }

    public int? CustomerId { get; set; }

    public decimal ReorderPoint { get; set; }

    public decimal PreferredQuantity { get; set; }

    public int IsLowStockWarningEnabled { get; set; }

    public decimal LowStockWarningQuantity { get; set; }

    public virtual Customer? Customer { get; set; }

    public virtual Product Product { get; set; } = null!;
}
