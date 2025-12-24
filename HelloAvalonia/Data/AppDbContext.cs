using System;
using System.Collections.Generic;
using HelloAvalonia.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace HelloAvalonia.Data;

public partial class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<ApplicationProperty> ApplicationProperties { get; set; }

    public virtual DbSet<Barcode> Barcodes { get; set; }

    public virtual DbSet<Company> Companies { get; set; }

    public virtual DbSet<Counter> Counters { get; set; }

    public virtual DbSet<Country> Countries { get; set; }

    public virtual DbSet<Currency> Currencies { get; set; }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<CustomerDiscount> CustomerDiscounts { get; set; }

    public virtual DbSet<Document> Documents { get; set; }

    public virtual DbSet<DocumentCategory> DocumentCategories { get; set; }

    public virtual DbSet<DocumentItem> DocumentItems { get; set; }

    public virtual DbSet<DocumentItemExpirationDate> DocumentItemExpirationDates { get; set; }

    public virtual DbSet<DocumentItemPriceView> DocumentItemPriceViews { get; set; }

    public virtual DbSet<DocumentItemTax> DocumentItemTaxes { get; set; }

    public virtual DbSet<DocumentType> DocumentTypes { get; set; }

    public virtual DbSet<FiscalItem> FiscalItems { get; set; }

    public virtual DbSet<FloorPlan> FloorPlans { get; set; }

    public virtual DbSet<FloorPlanTable> FloorPlanTables { get; set; }

    public virtual DbSet<LoyaltyCard> LoyaltyCards { get; set; }

    public virtual DbSet<Migration> Migrations { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<PaymentType> PaymentTypes { get; set; }

    public virtual DbSet<PaymentsView> PaymentsViews { get; set; }

    public virtual DbSet<PosOrder> PosOrders { get; set; }

    public virtual DbSet<PosOrderItem> PosOrderItems { get; set; }

    public virtual DbSet<PosPrinterSelection> PosPrinterSelections { get; set; }

    public virtual DbSet<PosPrinterSelectionSetting> PosPrinterSelectionSettings { get; set; }

    public virtual DbSet<PosPrinterSetting> PosPrinterSettings { get; set; }

    public virtual DbSet<PosVoid> PosVoids { get; set; }

    public virtual DbSet<PriceList> PriceLists { get; set; }

    public virtual DbSet<PriceListItem> PriceListItems { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<ProductComment> ProductComments { get; set; }

    public virtual DbSet<ProductGroup> ProductGroups { get; set; }

    public virtual DbSet<Promotion> Promotions { get; set; }

    public virtual DbSet<PromotionItem> PromotionItems { get; set; }

    public virtual DbSet<SecurityKey> SecurityKeys { get; set; }

    public virtual DbSet<StartingCash> StartingCashes { get; set; }

    public virtual DbSet<Stock> Stocks { get; set; }

    public virtual DbSet<StockControl> StockControls { get; set; }

    public virtual DbSet<Tax> Taxes { get; set; }

    public virtual DbSet<Template> Templates { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<VoidReason> VoidReasons { get; set; }

    public virtual DbSet<Warehouse> Warehouses { get; set; }

    public virtual DbSet<Zreport> Zreports { get; set; }

    /*protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Respect externally provided options (from ServiceProvider); fall back to default only when not configured.
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlite("Data Source=database/pos.db");
        }
    }*/

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ApplicationProperty>(entity =>
        {
            entity.HasKey(e => e.Name);

            entity.ToTable("ApplicationProperty");
        });

        modelBuilder.Entity<Barcode>(entity =>
        {
            entity.ToTable("Barcode");

            entity.HasIndex(e => e.ProductId, "IX_Barcode_Product");

            entity.HasOne(d => d.Product).WithMany(p => p.Barcodes).HasForeignKey(d => d.ProductId);
        });

        modelBuilder.Entity<Company>(entity =>
        {
            entity.ToTable("Company");

            entity.HasOne(d => d.Country).WithMany(p => p.Companies)
                .HasForeignKey(d => d.CountryId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<Counter>(entity =>
        {
            entity.HasKey(e => e.Name);

            entity.ToTable("Counter");
        });

        modelBuilder.Entity<Country>(entity =>
        {
            entity.ToTable("Country");
        });

        modelBuilder.Entity<Currency>(entity =>
        {
            entity.ToTable("Currency");
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.ToTable("Customer");

            entity.Property(e => e.DateCreated)
                .HasDefaultValueSql("DATETIME('now')")
                .HasColumnType("DATETIME");
            entity.Property(e => e.DateUpdated)
                .HasDefaultValueSql("DATETIME('now')")
                .HasColumnType("DATETIME");
            entity.Property(e => e.DueDatePeriod).HasColumnType("INT");
            entity.Property(e => e.IsCustomer)
                .HasDefaultValue(1)
                .HasColumnType("INTEGER (1)");
            entity.Property(e => e.IsEnabled)
                .HasDefaultValue(1)
                .HasColumnType("INTEGER (1)");
            entity.Property(e => e.IsSupplier)
                .HasDefaultValue(1)
                .HasColumnType("INTEGER (1)");
            entity.Property(e => e.IsTaxExempt).HasColumnType("INTEGER (1)");

            entity.HasOne(d => d.Country).WithMany(p => p.Customers).HasForeignKey(d => d.CountryId);

            entity.HasOne(d => d.PriceList).WithMany(p => p.Customers)
                .HasForeignKey(d => d.PriceListId)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<CustomerDiscount>(entity =>
        {
            entity.ToTable("CustomerDiscount");

            entity.HasIndex(e => new { e.CustomerId, e.Type, e.Uid }, "IX_CustomerDiscount_CustomerId_Type_Uid").IsUnique();

            entity.HasIndex(e => e.CustomerId, "IX_CustomerDiscount_CustomerId");

            entity.Property(e => e.Type).HasColumnType("INTEGER (1)");

            entity.HasOne(d => d.Customer).WithMany(p => p.CustomerDiscounts).HasForeignKey(d => d.CustomerId);
        });

        modelBuilder.Entity<Document>(entity =>
        {
            entity.ToTable("Document");

            entity.Property(e => e.Date).HasColumnType("DATE");
            entity.Property(e => e.DateCreated)
                .HasDefaultValueSql("datetime('now', 'localtime')")
                .HasColumnType("DATETIME");
            entity.Property(e => e.DateUpdated)
                .HasDefaultValueSql("datetime('now', 'localtime')")
                .HasColumnType("DATETIME");
            entity.Property(e => e.Discount).HasColumnType("NUMERIC");
            entity.Property(e => e.DiscountApplyRule).HasColumnType("INTEGER(1)");
            entity.Property(e => e.DiscountType).HasColumnType("INT");
            entity.Property(e => e.DueDate).HasColumnType("DATE");
            entity.Property(e => e.IsClockedOut).HasColumnType("INTEGER (1)");
            entity.Property(e => e.StockDate).HasColumnType("DATETIME");
            entity.Property(e => e.Total).HasColumnType("NUMERIC");

            entity.HasOne(d => d.Customer).WithMany(p => p.Documents).HasForeignKey(d => d.CustomerId);

            entity.HasOne(d => d.DocumentType).WithMany(p => p.Documents)
                .HasForeignKey(d => d.DocumentTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.User).WithMany(p => p.Documents)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Warehouse).WithMany(p => p.Documents)
                .HasForeignKey(d => d.WarehouseId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<DocumentCategory>(entity =>
        {
            entity.ToTable("DocumentCategory");
        });

        modelBuilder.Entity<DocumentItem>(entity =>
        {
            entity.ToTable("DocumentItem");

            entity.Property(e => e.Discount).HasColumnType("NUMERIC");
            entity.Property(e => e.DiscountApplyRule).HasColumnType("INTEGER(1)");
            entity.Property(e => e.ExpectedQuantity).HasColumnType("NUMERIC");
            entity.Property(e => e.Price).HasColumnType("NUMERIC");
            entity.Property(e => e.PriceAfterDiscount).HasColumnType("NUMERIC");
            entity.Property(e => e.PriceBeforeTax).HasColumnType("NUMERIC");
            entity.Property(e => e.PriceBeforeTaxAfterDiscount).HasColumnType("NUMERIC");
            entity.Property(e => e.ProductCost).HasColumnType("NUMERIC");
            entity.Property(e => e.Quantity).HasColumnType("NUMERIC");
            entity.Property(e => e.Total).HasColumnType("NUMERIC");
            entity.Property(e => e.TotalAfterDocumentDiscount).HasColumnType("NUMERIC");

            entity.HasOne(d => d.Document).WithMany(p => p.DocumentItems).HasForeignKey(d => d.DocumentId);

            entity.HasOne(d => d.Product).WithMany(p => p.DocumentItems)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<DocumentItemExpirationDate>(entity =>
        {
            entity.HasKey(e => e.DocumentItemId);

            entity.ToTable("DocumentItemExpirationDate");

            entity.HasIndex(e => e.DocumentItemId, "IX_DocumentItemExpirationdate").IsUnique();

            entity.Property(e => e.DocumentItemId).ValueGeneratedNever();
            entity.Property(e => e.ExpirationDate).HasColumnType("DATE");

            entity.HasOne(d => d.DocumentItem).WithOne(p => p.DocumentItemExpirationDate).HasForeignKey<DocumentItemExpirationDate>(d => d.DocumentItemId);
        });

        modelBuilder.Entity<DocumentItemPriceView>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("DocumentItemPriceView");
        });

        modelBuilder.Entity<DocumentItemTax>(entity =>
        {
            entity.HasKey(e => new { e.DocumentItemId, e.TaxId });

            entity.ToTable("DocumentItemTax");

            entity.HasOne(d => d.DocumentItem).WithMany(p => p.DocumentItemTaxes).HasForeignKey(d => d.DocumentItemId);

            entity.HasOne(d => d.Tax).WithMany(p => p.DocumentItemTaxes)
                .HasForeignKey(d => d.TaxId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<DocumentType>(entity =>
        {
            entity.ToTable("DocumentType");

            entity.Property(e => e.PriceType).HasColumnType("INT");
            entity.Property(e => e.StockDirection).HasColumnType("INTEGER (1)");

            entity.HasOne(d => d.DocumentCategory).WithMany(p => p.DocumentTypes)
                .HasForeignKey(d => d.DocumentCategoryId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Warehouse).WithMany(p => p.DocumentTypes)
                .HasForeignKey(d => d.WarehouseId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<FiscalItem>(entity =>
        {
            entity.HasKey(e => e.Plu);

            entity.ToTable("FiscalItem");

            entity.Property(e => e.Plu)
                .ValueGeneratedNever()
                .HasColumnName("PLU");
            entity.Property(e => e.Vat).HasColumnName("VAT");
        });

        modelBuilder.Entity<FloorPlan>(entity =>
        {
            entity.ToTable("FloorPlan");

            entity.Property(e => e.Color).HasDefaultValue("Transparent");
        });

        modelBuilder.Entity<FloorPlanTable>(entity =>
        {
            entity.ToTable("FloorPlanTable");

            entity.Property(e => e.IsRound).HasColumnType("INTEGER (1)");

            entity.HasOne(d => d.FloorPlan).WithMany(p => p.FloorPlanTables).HasForeignKey(d => d.FloorPlanId);
        });

        modelBuilder.Entity<LoyaltyCard>(entity =>
        {
            entity.ToTable("LoyaltyCard");

            entity.HasIndex(e => e.CustomerId, "IX_LoyaltyCard_CustomerId");

            entity.HasOne(d => d.Customer).WithMany(p => p.LoyaltyCards).HasForeignKey(d => d.CustomerId);
        });

        modelBuilder.Entity<Migration>(entity =>
        {
            entity.ToTable("Migration");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.Date).HasColumnType("DATETIME");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.ToTable("Payment");

            entity.Property(e => e.Amount).HasColumnType("NUMERIC");
            entity.Property(e => e.Date).HasColumnType("DATE");
            entity.Property(e => e.DateCreated)
                .HasDefaultValueSql("0")
                .HasColumnType("DATETIME");
            entity.Property(e => e.ZreportId).HasColumnName("ZReportId");

            entity.HasOne(d => d.Document).WithMany(p => p.Payments).HasForeignKey(d => d.DocumentId);

            entity.HasOne(d => d.PaymentType).WithMany(p => p.Payments)
                .HasForeignKey(d => d.PaymentTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.User).WithMany(p => p.Payments)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Zreport).WithMany(p => p.Payments).HasForeignKey(d => d.ZreportId);
        });

        modelBuilder.Entity<PaymentType>(entity =>
        {
            entity.ToTable("PaymentType");

            entity.Property(e => e.IsChangeAllowed)
                .HasDefaultValue(1)
                .HasColumnType("INTEGER (1)");
            entity.Property(e => e.IsCustomerRequired).HasColumnType("INTEGER (1)");
            entity.Property(e => e.IsEnabled)
                .HasDefaultValue(1)
                .HasColumnType("INTEGER (1)");
            entity.Property(e => e.IsFiscal)
                .HasDefaultValue(1)
                .HasColumnType("INTEGER (1)");
            entity.Property(e => e.IsQuickPayment)
                .HasDefaultValue(1)
                .HasColumnType("INTEGER (1)");
            entity.Property(e => e.IsSlipRequired).HasColumnType("INTEGER (1)");
            entity.Property(e => e.MarkAsPaid)
                .HasDefaultValue(1)
                .HasColumnType("INT(1)");
            entity.Property(e => e.OpenCashDrawer)
                .HasDefaultValue(1)
                .HasColumnType("INT (1)");
        });

        modelBuilder.Entity<PaymentsView>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("PaymentsView");
        });

        modelBuilder.Entity<PosOrder>(entity =>
        {
            entity.ToTable("PosOrder");

            entity.Property(e => e.Discount)
                .HasDefaultValueSql("0")
                .HasColumnType("NUMERIC");
            entity.Property(e => e.Total).HasColumnType("NUMERIC");

            entity.HasOne(d => d.Customer).WithMany(p => p.PosOrders)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(d => d.User).WithMany(p => p.PosOrders)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<PosOrderItem>(entity =>
        {
            entity.ToTable("PosOrderItem");

            entity.Property(e => e.DateCreated).HasColumnType("DATE");
            entity.Property(e => e.Discount)
                .HasDefaultValueSql("0")
                .HasColumnType("NUMERIC");
            entity.Property(e => e.DiscountType).HasColumnType("INT");
            entity.Property(e => e.IsFeatured).HasColumnType("INT (1)");
            entity.Property(e => e.IsLocked).HasColumnType("INTEGER (1)");
            entity.Property(e => e.Price).HasColumnType("NUMERIC");
            entity.Property(e => e.Quantity).HasColumnType("NUMERIC");

            entity.HasOne(d => d.PosOrder).WithMany(p => p.PosOrderItems).HasForeignKey(d => d.PosOrderId);

            entity.HasOne(d => d.Product).WithMany(p => p.PosOrderItems)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.VoidedByNavigation).WithMany(p => p.PosOrderItems).HasForeignKey(d => d.VoidedBy);
        });

        modelBuilder.Entity<PosPrinterSelection>(entity =>
        {
            entity.ToTable("PosPrinterSelection");

            entity.HasIndex(e => e.Key, "IX_PosPrinterSelection_Key").IsUnique();

            entity.Property(e => e.IsEnabled).HasColumnType("INTEGER (1)");
        });

        modelBuilder.Entity<PosPrinterSelectionSetting>(entity =>
        {
            entity.Property(e => e.BottomMargin)
                .HasDefaultValueSql("0")
                .HasColumnType("NUMERIC");
            entity.Property(e => e.CharacterSet).HasDefaultValue(-1);
            entity.Property(e => e.CodePage).HasDefaultValue(-1);
            entity.Property(e => e.CutPaper)
                .HasDefaultValue(1)
                .HasColumnType("INTEGER (1)");
            entity.Property(e => e.FeedLines).HasColumnType("INTEGER (3)");
            entity.Property(e => e.FontSizePercent)
                .HasDefaultValueSql("100.0")
                .HasColumnType("NUMERIC");
            entity.Property(e => e.FooterAlignment).HasColumnType("INT (1)");
            entity.Property(e => e.HeaderAlignment).HasColumnType("INT (1)");
            entity.Property(e => e.IsFormattingEnabled)
                .HasDefaultValue(1)
                .HasColumnType("INT (1)");
            entity.Property(e => e.LeftMargin)
                .HasDefaultValueSql("0")
                .HasColumnType("NUMERIC");
            entity.Property(e => e.NumberOfCopies)
                .HasDefaultValue(1)
                .HasColumnType("INT (1)");
            entity.Property(e => e.OpenCashDrawer)
                .HasDefaultValue(1)
                .HasColumnType("INT (1)");
            entity.Property(e => e.PaperWidth)
                .HasDefaultValue(32)
                .HasColumnType("INTEGER (5)");
            entity.Property(e => e.PrintBarcode)
                .HasDefaultValue(1)
                .HasColumnType("INTEGER (1)");
            entity.Property(e => e.PrintBitmap).HasColumnType("INTEGER (1)");
            entity.Property(e => e.PrintLogoFullWidth).HasColumnType("INTEGER (1)");
            entity.Property(e => e.PrinterType).HasColumnType("INT (1)");
            entity.Property(e => e.RightMargin)
                .HasDefaultValueSql("0")
                .HasColumnType("NUMERIC");
            entity.Property(e => e.TopMargin)
                .HasDefaultValueSql("0")
                .HasColumnType("NUMERIC");

            entity.HasOne(d => d.PosPrinterSelection).WithMany(p => p.PosPrinterSelectionSettings).HasForeignKey(d => d.PosPrinterSelectionId);
        });

        modelBuilder.Entity<PosPrinterSetting>(entity =>
        {
            entity.HasIndex(e => e.PrinterName, "IX_PosPrinterSettings_PrinterName").IsUnique();

            entity.Property(e => e.CharacterSet).HasDefaultValue(-1);
            entity.Property(e => e.CodePage).HasDefaultValue(-1);
            entity.Property(e => e.CutPaper)
                .HasDefaultValue(1)
                .HasColumnType("INTEGER(1)");
            entity.Property(e => e.FeedLines).HasColumnType("INTEGER (3)");
            entity.Property(e => e.FooterAlignment).HasColumnType("INT(1)");
            entity.Property(e => e.HeaderAlignment).HasColumnType("INT(1)");
            entity.Property(e => e.IsFormattingEnabled)
                .HasDefaultValue(1)
                .HasColumnType("INT(1)");
            entity.Property(e => e.NumberOfCopies)
                .HasDefaultValue(1)
                .HasColumnType("INT(1)");
            entity.Property(e => e.OpenCashDrawer)
                .HasDefaultValue(1)
                .HasColumnType("INT (1)");
            entity.Property(e => e.PaperWidth)
                .HasDefaultValue(32)
                .HasColumnType("INTEGER (5)");
            entity.Property(e => e.PrintBitmap).HasColumnType("INTEGER(1)");
            entity.Property(e => e.PrinterType).HasColumnType("INT(1)");
        });

        modelBuilder.Entity<PosVoid>(entity =>
        {
            entity.ToTable("PosVoid");

            entity.Property(e => e.DateCreated).HasColumnType("DATETIME");
            entity.Property(e => e.DateVoided).HasColumnType("DATETIME");
            entity.Property(e => e.Discount).HasColumnType("NUMERIC");
            entity.Property(e => e.Price).HasColumnType("NUMERIC");
            entity.Property(e => e.Quantity).HasColumnType("NUMERIC");
            entity.Property(e => e.Total).HasColumnType("NUMERIC");

            entity.HasOne(d => d.Product).WithMany(p => p.PosVoids)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(d => d.User).WithMany(p => p.PosVoidUsers)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(d => d.VoidedByNavigation).WithMany(p => p.PosVoidVoidedByNavigations)
                .HasForeignKey(d => d.VoidedBy)
                .OnDelete(DeleteBehavior.SetNull);
        });

        modelBuilder.Entity<PriceList>(entity =>
        {
            entity.ToTable("PriceList");

            entity.Property(e => e.ArePromotionsAllowed)
                .HasDefaultValue(1)
                .HasColumnType("INTEGER(1)");
            entity.Property(e => e.DateCreated)
                .HasDefaultValueSql("datetime('now', 'localtime')")
                .HasColumnType("DATETIME");
            entity.Property(e => e.DateUpdated)
                .HasDefaultValueSql("datetime('now', 'localtime')")
                .HasColumnType("DATETIME");
            entity.Property(e => e.IsActive).HasColumnType("INTEGER(1)");
        });

        modelBuilder.Entity<PriceListItem>(entity =>
        {
            entity.ToTable("PriceListItem");

            entity.Property(e => e.Markup)
                .HasDefaultValueSql("0")
                .HasColumnType("NUMERIC");
            entity.Property(e => e.Price)
                .HasDefaultValueSql("0")
                .HasColumnType("NUMERIC");

            entity.HasOne(d => d.PriceList).WithMany(p => p.PriceListItems).HasForeignKey(d => d.PriceListId);

            entity.HasOne(d => d.Product).WithMany(p => p.PriceListItems).HasForeignKey(d => d.ProductId);
        });

        modelBuilder.Entity<Product>(entity =>
        {
            entity.ToTable("Product");

            entity.Property(e => e.AgeRestriction).HasColumnType("NUMERIC");
            entity.Property(e => e.Color).HasDefaultValue("Transparent");
            entity.Property(e => e.Cost).HasColumnType("NUMERIC");
            entity.Property(e => e.DateCreated)
                .HasDefaultValueSql("DATETIME ('now','localtime')")
                .HasColumnType("DATETIME");
            entity.Property(e => e.DateUpdated)
                .HasDefaultValueSql("DATETIME ('now','localtime')")
                .HasColumnType("DATETIME");
            entity.Property(e => e.IsEnabled)
                .HasDefaultValue(1)
                .HasColumnType("INTEGER (1)");
            entity.Property(e => e.IsPriceChangeAllowed).HasColumnType("INTEGER (1)");
            entity.Property(e => e.IsService).HasColumnType("INTEGER (1)");
            entity.Property(e => e.IsTaxInclusivePrice)
                .HasDefaultValue(1)
                .HasColumnType("INTEGER (1)");
            entity.Property(e => e.IsUsingDefaultQuantity)
                .HasDefaultValue(1)
                .HasColumnType("INTEGER (1)");
            entity.Property(e => e.LastPurchasePrice).HasColumnType("NUMERIC");
            entity.Property(e => e.Markup).HasColumnType("NUMERIC");
            entity.Property(e => e.Plu).HasColumnName("PLU");
            entity.Property(e => e.Rank).HasColumnType("NUMERIC");

            entity.HasOne(d => d.Currency).WithMany(p => p.Products).HasForeignKey(d => d.CurrencyId);

            entity.HasOne(d => d.ProductGroup).WithMany(p => p.Products)
                .HasForeignKey(d => d.ProductGroupId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasMany(d => d.Taxes).WithMany(p => p.Products)
                .UsingEntity<Dictionary<string, object>>(
                    "ProductTax",
                    r => r.HasOne<Tax>().WithMany()
                        .HasForeignKey("TaxId")
                        .OnDelete(DeleteBehavior.ClientSetNull),
                    l => l.HasOne<Product>().WithMany().HasForeignKey("ProductId"),
                    j =>
                    {
                        j.HasKey("ProductId", "TaxId");
                        j.ToTable("ProductTax");
                    });
        });

        modelBuilder.Entity<ProductComment>(entity =>
        {
            entity.ToTable("ProductComment");

            entity.HasOne(d => d.Product).WithMany(p => p.ProductComments).HasForeignKey(d => d.ProductId);
        });

        modelBuilder.Entity<ProductGroup>(entity =>
        {
            entity.ToTable("ProductGroup");

            entity.Property(e => e.Color).HasDefaultValue("Transparent");
            entity.Property(e => e.Rank).HasColumnType("INT");
        });

        modelBuilder.Entity<Promotion>(entity =>
        {
            entity.ToTable("Promotion");

            entity.Property(e => e.EndDate).HasColumnType("DATETIME");
            entity.Property(e => e.EndTime).HasColumnType("DATETIME");
            entity.Property(e => e.IsEnabled)
                .HasDefaultValue(1)
                .HasColumnType("INTEGER(1)");
            entity.Property(e => e.StartDate).HasColumnType("DATETIME");
            entity.Property(e => e.StartTime).HasColumnType("DATETIME");
        });

        modelBuilder.Entity<PromotionItem>(entity =>
        {
            entity.ToTable("PromotionItem");

            entity.Property(e => e.IsConditional)
                .HasDefaultValue(1)
                .HasColumnType("INTEGER(1)");
            entity.Property(e => e.Quantity)
                .HasDefaultValueSql("0")
                .HasColumnType("NUMERIC");
            entity.Property(e => e.QuantityLimit)
                .HasDefaultValueSql("0")
                .HasColumnType("NUMERIC");
            entity.Property(e => e.Value)
                .HasDefaultValueSql("0")
                .HasColumnType("NUMERIC");

            entity.HasOne(d => d.Promotion).WithMany(p => p.PromotionItems).HasForeignKey(d => d.PromotionId);
        });

        modelBuilder.Entity<SecurityKey>(entity =>
        {
            entity.HasKey(e => e.Name);

            entity.ToTable("SecurityKey");
        });

        modelBuilder.Entity<StartingCash>(entity =>
        {
            entity.ToTable("StartingCash");

            entity.HasIndex(e => e.Id, "IX_StartingCash_Id").IsUnique();

            entity.Property(e => e.Amount).HasColumnType("NUMERIC");
            entity.Property(e => e.DateCreated).HasColumnType("DATETIME");
            entity.Property(e => e.ZreportNumber).HasColumnName("ZReportNumber");

            entity.HasOne(d => d.User).WithMany(p => p.StartingCashes)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<Stock>(entity =>
        {
            entity.ToTable("Stock");

            entity.HasIndex(e => e.ProductId, "IX_Stock_ProductId");

            entity.Property(e => e.Quantity).HasColumnType("NUMERIC");

            entity.HasOne(d => d.Product).WithMany(p => p.Stocks).HasForeignKey(d => d.ProductId);

            entity.HasOne(d => d.Warehouse).WithMany(p => p.Stocks).HasForeignKey(d => d.WarehouseId);
        });

        modelBuilder.Entity<StockControl>(entity =>
        {
            entity.ToTable("StockControl");

            entity.HasIndex(e => e.ProductId, "IX_StockControl_Product").IsUnique();

            entity.Property(e => e.IsLowStockWarningEnabled)
                .HasDefaultValue(1)
                .HasColumnType("INTEGER (1)");
            entity.Property(e => e.LowStockWarningQuantity)
                .HasDefaultValueSql("0")
                .HasColumnType("NUMERIC");
            entity.Property(e => e.PreferredQuantity)
                .HasDefaultValueSql("0")
                .HasColumnType("NUMERIC");
            entity.Property(e => e.ReorderPoint)
                .HasDefaultValueSql("0")
                .HasColumnType("NUMERIC");

            entity.HasOne(d => d.Customer).WithMany(p => p.StockControls)
                .HasForeignKey(d => d.CustomerId)
                .OnDelete(DeleteBehavior.SetNull);

            entity.HasOne(d => d.Product).WithOne(p => p.StockControl).HasForeignKey<StockControl>(d => d.ProductId);
        });

        modelBuilder.Entity<Tax>(entity =>
        {
            entity.ToTable("Tax");

            entity.Property(e => e.IsEnabled)
                .HasDefaultValue(1)
                .HasColumnType("INTEGER (1)");
            entity.Property(e => e.IsFixed).HasColumnType("INTEGER (1)");
            entity.Property(e => e.IsTaxOnTotal).HasColumnType("INTEGER (1)");
            entity.Property(e => e.Rate).HasColumnType("NUMERIC");
        });

        modelBuilder.Entity<Template>(entity =>
        {
            entity.ToTable("Template");

            entity.HasIndex(e => e.Name, "UK_TemplateName").IsUnique();
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("User");

            entity.HasIndex(e => e.Email, "UX_UserEmail").IsUnique();

            entity.Property(e => e.IsEnabled)
                .HasDefaultValue(1)
                .HasColumnType("INT (1)");
        });

        modelBuilder.Entity<VoidReason>(entity =>
        {
            entity.ToTable("VoidReason");

            entity.Property(e => e.DateCreated).HasColumnType("DATETIME");
        });

        modelBuilder.Entity<Warehouse>(entity =>
        {
            entity.ToTable("Warehouse");
        });

        modelBuilder.Entity<Zreport>(entity =>
        {
            entity.ToTable("ZReport");

            entity.Property(e => e.DateCreated)
                .HasDefaultValueSql("datetime('now', 'localtime')")
                .HasColumnType("DATETIME");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
