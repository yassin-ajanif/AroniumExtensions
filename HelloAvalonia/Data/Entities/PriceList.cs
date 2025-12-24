using System;
using System.Collections.Generic;

namespace HelloAvalonia.Data.Entities;

public partial class PriceList
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int ArePromotionsAllowed { get; set; }

    public int IsActive { get; set; }

    public DateTime DateCreated { get; set; }

    public DateTime DateUpdated { get; set; }

    public virtual ICollection<Customer> Customers { get; set; } = new List<Customer>();

    public virtual ICollection<PriceListItem> PriceListItems { get; set; } = new List<PriceListItem>();
}
