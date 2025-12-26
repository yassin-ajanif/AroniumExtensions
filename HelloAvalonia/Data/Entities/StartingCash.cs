using System;
using System.Collections.Generic;

namespace AroniumFactures.Data.Entities;

public partial class StartingCash
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public decimal Amount { get; set; }

    public string? Description { get; set; }

    public int StartingCashType { get; set; }

    public int? ZreportNumber { get; set; }

    public DateTime DateCreated { get; set; }

    public virtual User User { get; set; } = null!;
}
