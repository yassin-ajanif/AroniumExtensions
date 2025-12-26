using System;
using System.Collections.Generic;

namespace AroniumFactures.Data.Entities;

public partial class SecurityKey
{
    public string Name { get; set; } = null!;

    public int? Level { get; set; }
}
