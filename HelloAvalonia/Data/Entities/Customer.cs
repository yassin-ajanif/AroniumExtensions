using System;
using System.Collections.Generic;

namespace AroniumFactures.Data.Entities;

public partial class Customer
{
    public int Id { get; set; }

    public string? Code { get; set; }

    public string Name { get; set; } = null!;

    public string? TaxNumber { get; set; }

    public string? Address { get; set; }

    public string? PostalCode { get; set; }

    public string? City { get; set; }

    public int? CountryId { get; set; }

    public DateTime DateCreated { get; set; }

    public DateTime DateUpdated { get; set; }

    public string? Email { get; set; }

    public string? PhoneNumber { get; set; }

    public int IsEnabled { get; set; }

    public int IsCustomer { get; set; }

    public int IsSupplier { get; set; }

    public int DueDatePeriod { get; set; }

    public string? StreetName { get; set; }

    public string? AdditionalStreetName { get; set; }

    public string? BuildingNumber { get; set; }

    public string? PlotIdentification { get; set; }

    public string? CitySubdivisionName { get; set; }

    public string? CountrySubentity { get; set; }

    public int IsTaxExempt { get; set; }

    public int? PriceListId { get; set; }

    public virtual Country? Country { get; set; }

    public virtual ICollection<CustomerDiscount> CustomerDiscounts { get; set; } = new List<CustomerDiscount>();

    public virtual ICollection<Document> Documents { get; set; } = new List<Document>();

    public virtual ICollection<LoyaltyCard> LoyaltyCards { get; set; } = new List<LoyaltyCard>();

    public virtual ICollection<PosOrder> PosOrders { get; set; } = new List<PosOrder>();

    public virtual PriceList? PriceList { get; set; }

    public virtual ICollection<StockControl> StockControls { get; set; } = new List<StockControl>();
}
