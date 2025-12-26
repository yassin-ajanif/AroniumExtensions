using System;
using System.Collections.Generic;

namespace AroniumFactures.Data.Entities;

public partial class Document
{
    public int Id { get; set; }

    public string Number { get; set; } = null!;

    public int UserId { get; set; }

    public int? CustomerId { get; set; }

    public string? OrderNumber { get; set; }

    public DateTime Date { get; set; }

    public DateTime StockDate { get; set; }

    public int Total { get; set; }

    public int IsClockedOut { get; set; }

    public int DocumentTypeId { get; set; }

    public int WarehouseId { get; set; }

    public string? ReferenceDocumentNumber { get; set; }

    public DateTime DateCreated { get; set; }

    public DateTime DateUpdated { get; set; }

    public string? InternalNote { get; set; }

    public string? Note { get; set; }

    public DateTime? DueDate { get; set; }

    public int Discount { get; set; }

    public int DiscountType { get; set; }

    public int PaidStatus { get; set; }

    public int DiscountApplyRule { get; set; }

    public int ServiceType { get; set; }

    public virtual Customer? Customer { get; set; }

    public virtual ICollection<DocumentItem> DocumentItems { get; set; } = new List<DocumentItem>();

    public virtual DocumentType DocumentType { get; set; } = null!;

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual User User { get; set; } = null!;

    public virtual Warehouse Warehouse { get; set; } = null!;
}
