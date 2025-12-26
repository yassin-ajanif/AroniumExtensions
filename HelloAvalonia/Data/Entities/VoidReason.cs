using System;
using System.Collections.Generic;

namespace AroniumFactures.Data.Entities;

public partial class VoidReason
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int Rank { get; set; }

    public DateTime DateCreated { get; set; }
}
