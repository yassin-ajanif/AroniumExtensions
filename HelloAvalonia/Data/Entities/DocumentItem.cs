using System;
using System.Collections.Generic;

namespace HelloAvalonia.Data.Entities;

public partial class DocumentItem
{
    public int Id { get; set; }

    public int DocumentId { get; set; }

    public int ProductId { get; set; }

    public decimal Quantity { get; set; }

    public decimal ExpectedQuantity { get; set; }

    public decimal PriceBeforeTax { get; set; }

    public decimal Price { get; set; }

    public decimal Discount { get; set; }

    public int DiscountType { get; set; }

    public decimal ProductCost { get; set; }

    public decimal PriceBeforeTaxAfterDiscount { get; set; }

    public decimal PriceAfterDiscount { get; set; }

    public decimal Total { get; set; }

    public decimal TotalAfterDocumentDiscount { get; set; }

    public int DiscountApplyRule { get; set; }

    public virtual Document Document { get; set; } = null!;

    public virtual DocumentItemExpirationDate? DocumentItemExpirationDate { get; set; }

    public virtual ICollection<DocumentItemTax> DocumentItemTaxes { get; set; } = new List<DocumentItemTax>();

    public virtual Product Product { get; set; } = null!;
}
