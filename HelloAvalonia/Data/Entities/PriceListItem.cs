using System;
using System.Collections.Generic;

namespace HelloAvalonia.Data.Entities;

public partial class PriceListItem
{
    public int Id { get; set; }

    public int PriceListId { get; set; }

    public int ProductId { get; set; }

    public decimal Price { get; set; }

    public decimal Markup { get; set; }

    public virtual PriceList PriceList { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;
}
