using System;
using System.Collections.Generic;

namespace AroniumFactures.Data.Entities;

public partial class Payment
{
    public int Id { get; set; }

    public int DocumentId { get; set; }

    public int PaymentTypeId { get; set; }

    public int Amount { get; set; }

    public DateTime? Date { get; set; }

    public int UserId { get; set; }

    public int? ZreportId { get; set; }

    public DateTime DateCreated { get; set; }

    public virtual Document Document { get; set; } = null!;

    public virtual PaymentType PaymentType { get; set; } = null!;

    public virtual User User { get; set; } = null!;

    public virtual Zreport? Zreport { get; set; }
}
