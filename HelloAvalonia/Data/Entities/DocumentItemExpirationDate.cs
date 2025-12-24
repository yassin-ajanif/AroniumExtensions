using System;
using System.Collections.Generic;

namespace HelloAvalonia.Data.Entities;

public partial class DocumentItemExpirationDate
{
    public int DocumentItemId { get; set; }

    public DateTime ExpirationDate { get; set; }

    public virtual DocumentItem DocumentItem { get; set; } = null!;
}
