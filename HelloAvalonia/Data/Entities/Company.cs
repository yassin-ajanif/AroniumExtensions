using System;
using System.Collections.Generic;

namespace HelloAvalonia.Data.Entities;

public partial class Company
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Address { get; set; }

    public string? PostalCode { get; set; }

    public string? City { get; set; }

    public int CountryId { get; set; }

    public string? TaxNumber { get; set; }

    public string? Email { get; set; }

    public string? PhoneNumber { get; set; }

    public byte[]? Logo { get; set; }

    public string? BankAccountNumber { get; set; }

    public string? BankDetails { get; set; }

    public string? StreetName { get; set; }

    public string? AdditionalStreetName { get; set; }

    public string? BuildingNumber { get; set; }

    public string? PlotIdentification { get; set; }

    public string? CitySubdivisionName { get; set; }

    public string? CountrySubentity { get; set; }

    public virtual Country Country { get; set; } = null!;
}
