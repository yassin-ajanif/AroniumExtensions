using System;
using System.Collections.Generic;

namespace AroniumFactures.Data.Entities;

public partial class PosPrinterSetting
{
    public int Id { get; set; }

    public string PrinterName { get; set; } = null!;

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
}
