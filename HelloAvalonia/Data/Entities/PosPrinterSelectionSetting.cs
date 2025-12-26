using System;
using System.Collections.Generic;

namespace AroniumFactures.Data.Entities;

public partial class PosPrinterSelectionSetting
{
    public int Id { get; set; }

    public int PosPrinterSelectionId { get; set; }

    public int PaperWidth { get; set; }

    public string? Header { get; set; }

    public string? Footer { get; set; }

    public int FeedLines { get; set; }

    public int CutPaper { get; set; }

    public int PrintBitmap { get; set; }

    public int OpenCashDrawer { get; set; }

    public string? CashDrawerCommand { get; set; }

    public int HeaderAlignment { get; set; }

    public int FooterAlignment { get; set; }

    public int IsFormattingEnabled { get; set; }

    public int PrinterType { get; set; }

    public int NumberOfCopies { get; set; }

    public int CodePage { get; set; }

    public int CharacterSet { get; set; }

    public int Margin { get; set; }

    public decimal LeftMargin { get; set; }

    public decimal TopMargin { get; set; }

    public decimal RightMargin { get; set; }

    public decimal BottomMargin { get; set; }

    public int PrintBarcode { get; set; }

    public string? FontName { get; set; }

    public decimal FontSizePercent { get; set; }

    public int PrintLogoFullWidth { get; set; }

    public virtual PosPrinterSelection PosPrinterSelection { get; set; } = null!;
}
