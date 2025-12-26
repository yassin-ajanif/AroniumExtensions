using System;
using System.Collections.Generic;

namespace AroniumFactures.Data.Entities;

public partial class PromotionItem
{
    public int Id { get; set; }

    public int PromotionId { get; set; }

    public int Uid { get; set; }

    public int DiscountType { get; set; }

    public int PriceType { get; set; }

    public decimal Value { get; set; }

    public int IsConditional { get; set; }

    public decimal Quantity { get; set; }

    public int ConditionType { get; set; }

    public decimal QuantityLimit { get; set; }

    public virtual Promotion Promotion { get; set; } = null!;
}
