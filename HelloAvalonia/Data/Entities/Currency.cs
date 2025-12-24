using System;
using System.Collections.Generic;

namespace HelloAvalonia.Data.Entities;

public partial class Currency
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Code { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
