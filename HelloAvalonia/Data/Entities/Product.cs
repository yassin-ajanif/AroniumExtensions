using System;
using System.Collections.Generic;

namespace HelloAvalonia.Data.Entities;

public partial class Product
{
    public int Id { get; set; }

    public int? ProductGroupId { get; set; }

    public string Name { get; set; } = null!;

    public string? Code { get; set; }

    public int? Plu { get; set; }

    public string? MeasurementUnit { get; set; }

    public double Price { get; set; }

    public int? IsTaxInclusivePrice { get; set; }

    public int? CurrencyId { get; set; }

    public int IsPriceChangeAllowed { get; set; }

    public int IsService { get; set; }

    public int IsUsingDefaultQuantity { get; set; }

    public int IsEnabled { get; set; }

    public string? Description { get; set; }

    public DateTime DateCreated { get; set; }

    public DateTime DateUpdated { get; set; }

    public int Cost { get; set; }

    public int Markup { get; set; }

    public byte[]? Image { get; set; }

    public string Color { get; set; } = null!;

    public decimal? AgeRestriction { get; set; }

    public int LastPurchasePrice { get; set; }

    public int Rank { get; set; }

    public virtual ICollection<Barcode> Barcodes { get; set; } = new List<Barcode>();

    public virtual Currency? Currency { get; set; }

    public virtual ICollection<DocumentItem> DocumentItems { get; set; } = new List<DocumentItem>();

    public virtual ICollection<PosOrderItem> PosOrderItems { get; set; } = new List<PosOrderItem>();

    public virtual ICollection<PosVoid> PosVoids { get; set; } = new List<PosVoid>();

    public virtual ICollection<PriceListItem> PriceListItems { get; set; } = new List<PriceListItem>();

    public virtual ICollection<ProductComment> ProductComments { get; set; } = new List<ProductComment>();

    public virtual ProductGroup? ProductGroup { get; set; }

    public virtual StockControl? StockControl { get; set; }

    public virtual ICollection<Stock> Stocks { get; set; } = new List<Stock>();

    public virtual ICollection<Tax> Taxes { get; set; } = new List<Tax>();
}
