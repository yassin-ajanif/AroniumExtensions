using System;
using System.Collections.Generic;

namespace HelloAvalonia.Data.Entities;

public partial class ApplicationProperty
{
    public string Name { get; set; } = null!;

    public string? Value { get; set; }
}
