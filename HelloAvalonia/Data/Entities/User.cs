using System;
using System.Collections.Generic;

namespace AroniumFactures.Data.Entities;

public partial class User
{
    public int Id { get; set; }

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string? Username { get; set; }

    public string Password { get; set; } = null!;

    public int AccessLevel { get; set; }

    public int IsEnabled { get; set; }

    public string? Email { get; set; }

    public virtual ICollection<Document> Documents { get; set; } = new List<Document>();

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual ICollection<PosOrderItem> PosOrderItems { get; set; } = new List<PosOrderItem>();

    public virtual ICollection<PosOrder> PosOrders { get; set; } = new List<PosOrder>();

    public virtual ICollection<PosVoid> PosVoidUsers { get; set; } = new List<PosVoid>();

    public virtual ICollection<PosVoid> PosVoidVoidedByNavigations { get; set; } = new List<PosVoid>();

    public virtual ICollection<StartingCash> StartingCashes { get; set; } = new List<StartingCash>();
}
