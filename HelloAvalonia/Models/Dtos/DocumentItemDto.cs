namespace AroniumFactures.Models;

public class DocumentItemDto
{
    public int Id { get; set; }
    public string ProductCode { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public string UnitOfMeasure { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public decimal PriceBeforeTax { get; set; }
    public decimal Tax { get; set; }
    public decimal TaxRate { get; set; }
    public decimal Price { get; set; }
    public decimal TotalBeforeDiscount { get; set; }
    public decimal Discount { get; set; }
    public int DiscountType { get; set; }
    public decimal Total { get; set; }
}

