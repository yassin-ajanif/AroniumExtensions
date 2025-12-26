using System;
using System.Collections.Generic;

namespace AroniumFactures.Models;

public class DocumentWithItemsDto
{
    public string DocumentNumber { get; set; } = string.Empty;
    public string? CustomerName { get; set; }
    public DateTime Date { get; set; }
    public DateTime? DueDate { get; set; }
    public string? PaymentTypeName { get; set; }
    public string? DocumentTypeCode { get; set; }
    public string? DocumentTypeName { get; set; }
    public int DocumentDiscount { get; set; }
    public int DocumentDiscountType { get; set; }
    public List<DocumentItemDto> Items { get; set; } = new();
}

