using System;
using System.Collections.Generic;

namespace HelloAvalonia.Data.Entities;

public partial class DocumentItem
{
    public int Id { get; set; }

    public int DocumentId { get; set; }

    public int ProductId { get; set; }

    public int Quantity { get; set; }

    public int ExpectedQuantity { get; set; }

    public int PriceBeforeTax { get; set; }

    public int Price { get; set; }

    public int Discount { get; set; }

    public int DiscountType { get; set; }

    public int ProductCost { get; set; }

    public int PriceBeforeTaxAfterDiscount { get; set; }

    public int PriceAfterDiscount { get; set; }

    public int Total { get; set; }

    public int TotalAfterDocumentDiscount { get; set; }

    public int DiscountApplyRule { get; set; }

    public virtual Document Document { get; set; } = null!;

    public virtual DocumentItemExpirationDate? DocumentItemExpirationDate { get; set; }

    public virtual ICollection<DocumentItemTax> DocumentItemTaxes { get; set; } = new List<DocumentItemTax>();

    public virtual Product Product { get; set; } = null!;
}
