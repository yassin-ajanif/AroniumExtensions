using System;
using System.Collections.Generic;

namespace AroniumFactures.Data.Entities;

public partial class PosOrderItem
{
    public int Id { get; set; }

    public int PosOrderId { get; set; }

    public int ProductId { get; set; }

    public int RoundNumber { get; set; }

    public decimal Quantity { get; set; }

    public decimal Price { get; set; }

    public int IsLocked { get; set; }

    public decimal Discount { get; set; }

    public int DiscountType { get; set; }

    public int IsFeatured { get; set; }

    public int? VoidedBy { get; set; }

    public string? Comment { get; set; }

    public DateTime DateCreated { get; set; }

    public string? Bundle { get; set; }

    public int DiscountAppliedType { get; set; }

    public virtual PosOrder PosOrder { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;

    public virtual User? VoidedByNavigation { get; set; }
}
