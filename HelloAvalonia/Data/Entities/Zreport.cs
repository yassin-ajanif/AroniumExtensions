using System;
using System.Collections.Generic;

namespace HelloAvalonia.Data.Entities;

public partial class Zreport
{
    public int Id { get; set; }

    public int Number { get; set; }

    public int FromDocumentId { get; set; }

    public int ToDocumentId { get; set; }

    public DateTime DateCreated { get; set; }

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
