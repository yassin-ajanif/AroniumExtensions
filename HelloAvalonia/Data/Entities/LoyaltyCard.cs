using System;
using System.Collections.Generic;

namespace HelloAvalonia.Data.Entities;

public partial class LoyaltyCard
{
    public int Id { get; set; }

    public int CustomerId { get; set; }

    public string? CardNumber { get; set; }

    public virtual Customer Customer { get; set; } = null!;
}
