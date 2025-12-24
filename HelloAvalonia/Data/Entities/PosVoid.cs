using System;
using System.Collections.Generic;

namespace HelloAvalonia.Data.Entities;

public partial class PosVoid
{
    public int Id { get; set; }

    public string OrderNumber { get; set; } = null!;

    public int? UserId { get; set; }

    public string UserName { get; set; } = null!;

    public int? ProductId { get; set; }

    public string ProductName { get; set; } = null!;

    public int RoundNumber { get; set; }

    public int Quantity { get; set; }

    public int Price { get; set; }

    public int Discount { get; set; }

    public int DiscountType { get; set; }

    public int Total { get; set; }

    public int IsConfirmed { get; set; }

    public string? Reason { get; set; }

    public int? VoidedBy { get; set; }

    public string? VoidedByName { get; set; }

    public string? Bundle { get; set; }

    public DateTime DateCreated { get; set; }

    public DateTime DateVoided { get; set; }

    public virtual Product? Product { get; set; }

    public virtual User? User { get; set; }

    public virtual User? VoidedByNavigation { get; set; }
}
