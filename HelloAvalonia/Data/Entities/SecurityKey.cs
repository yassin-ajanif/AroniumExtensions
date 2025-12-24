using System;
using System.Collections.Generic;

namespace HelloAvalonia.Data.Entities;

public partial class SecurityKey
{
    public string Name { get; set; } = null!;

    public int? Level { get; set; }
}
