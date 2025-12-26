using System;
using System.Collections.Generic;

namespace AroniumFactures.Data.Entities;

public partial class DocumentType
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Code { get; set; } = null!;

    public int DocumentCategoryId { get; set; }

    public int WarehouseId { get; set; }

    public int StockDirection { get; set; }

    public int EditorType { get; set; }

    public string? PrintTemplate { get; set; }

    public int PriceType { get; set; }

    public string? LanguageKey { get; set; }

    public virtual DocumentCategory DocumentCategory { get; set; } = null!;

    public virtual ICollection<Document> Documents { get; set; } = new List<Document>();

    public virtual Warehouse Warehouse { get; set; } = null!;
}
