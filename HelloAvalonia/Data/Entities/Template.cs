using System;
using System.Collections.Generic;

namespace AroniumFactures.Data.Entities;

public partial class Template
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Value { get; set; } = null!;
}
