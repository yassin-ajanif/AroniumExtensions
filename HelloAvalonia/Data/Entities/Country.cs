using System;
using System.Collections.Generic;

namespace HelloAvalonia.Data.Entities;

public partial class Country
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Code { get; set; }

    public virtual ICollection<Company> Companies { get; set; } = new List<Company>();

    public virtual ICollection<Customer> Customers { get; set; } = new List<Customer>();
}
