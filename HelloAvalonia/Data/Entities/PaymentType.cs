using System;
using System.Collections.Generic;

namespace HelloAvalonia.Data.Entities;

public partial class PaymentType
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Code { get; set; }

    public int IsCustomerRequired { get; set; }

    public int IsFiscal { get; set; }

    public int IsSlipRequired { get; set; }

    public int IsChangeAllowed { get; set; }

    public int Ordinal { get; set; }

    public int IsEnabled { get; set; }

    public int IsQuickPayment { get; set; }

    public int OpenCashDrawer { get; set; }

    public string? ShortcutKey { get; set; }

    public int MarkAsPaid { get; set; }

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
