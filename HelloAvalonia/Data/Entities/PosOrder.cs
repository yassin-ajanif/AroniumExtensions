using System;
using System.Collections.Generic;

namespace HelloAvalonia.Data.Entities;

public partial class PosOrder
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string Number { get; set; } = null!;

    public decimal Discount { get; set; }

    public int DiscountType { get; set; }

    public decimal? Total { get; set; }

    public int? CustomerId { get; set; }

    public int ServiceType { get; set; }

    public virtual Customer? Customer { get; set; }

    public virtual ICollection<PosOrderItem> PosOrderItems { get; set; } = new List<PosOrderItem>();

    public virtual User User { get; set; } = null!;
}
