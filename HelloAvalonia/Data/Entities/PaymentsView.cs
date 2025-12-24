using System;
using System.Collections.Generic;

namespace HelloAvalonia.Data.Entities;

public partial class PaymentsView
{
    public int? DocumentId { get; set; }

    public string? PaymentTypes { get; set; }
}
