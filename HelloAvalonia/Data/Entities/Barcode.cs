using System;
using System.Collections.Generic;

namespace AroniumFactures.Data.Entities;

public partial class Barcode
{
    public int Id { get; set; }

    public int ProductId { get; set; }

    public string Value { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;
}
