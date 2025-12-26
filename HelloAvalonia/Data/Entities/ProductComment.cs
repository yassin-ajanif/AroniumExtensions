using System;
using System.Collections.Generic;

namespace AroniumFactures.Data.Entities;

public partial class ProductComment
{
    public int Id { get; set; }

    public int ProductId { get; set; }

    public string Comment { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;
}
