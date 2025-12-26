using System;
using System.Collections.Generic;

namespace AroniumFactures.Data.Entities;

public partial class Counter
{
    public string Name { get; set; } = null!;

    public int Value { get; set; }
}
