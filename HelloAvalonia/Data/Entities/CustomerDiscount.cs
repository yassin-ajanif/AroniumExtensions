using System;
using System.Collections.Generic;

namespace AroniumFactures.Data.Entities;

public partial class CustomerDiscount
{
    public int Id { get; set; }

    public int CustomerId { get; set; }

    public int Type { get; set; }

    public int? Uid { get; set; }

    public double Value { get; set; }

    public virtual Customer Customer { get; set; } = null!;
}
