using System;
using System.Collections.Generic;

namespace AroniumFactures.Data.Entities;

public partial class DocumentItemPriceView
{
    public int? DocumentItemId { get; set; }

    public int? Price { get; set; }
}
