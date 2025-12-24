using System;
using System.Collections.Generic;

namespace HelloAvalonia.Data.Entities;

public partial class ProductGroup
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int? ParentGroupId { get; set; }

    public string Color { get; set; } = null!;

    public byte[]? Image { get; set; }

    public int Rank { get; set; }

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
