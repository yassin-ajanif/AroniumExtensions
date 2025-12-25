using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HelloAvalonia.Data;
using HelloAvalonia.Data.Entities;
using HelloAvalonia.Models;
using Microsoft.EntityFrameworkCore;

namespace HelloAvalonia.Repositories;

public class DocumentRepository : IDocumentRepository
{
    private readonly AppDbContext _context;

    public DocumentRepository(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Gets document with items and discount info by document number
    /// </summary>
    public async Task<DocumentWithItemsDto?> GetDocumentWithItemsByNumberAsync(string documentNumber)
    {
        var document = await _context.Documents
            .AsNoTracking()
            .Include(d => d.Customer)
            .Include(d => d.DocumentType)
            .Include(d => d.Payments)
                .ThenInclude(p => p.PaymentType)
            .Include(d => d.DocumentItems)
                .ThenInclude(di => di.Product)
            .Include(d => d.DocumentItems)
                .ThenInclude(di => di.DocumentItemTaxes)
                    .ThenInclude(dit => dit.Tax)
            .Where(d => d.Number == documentNumber)
            .FirstOrDefaultAsync();
        
        if (document == null)
            return null;
        
        // Get all payment types and concatenate with commas
        var paymentTypeNames = document.Payments
            .Where(p => p.PaymentType != null)
            .Select(p => p.PaymentType.Code ?? p.PaymentType.Name)
            .Where(name => !string.IsNullOrEmpty(name))
            .Distinct()  // Remove duplicates if same payment type appears multiple times
            .ToList();

        var paymentTypeName = paymentTypeNames.Any() 
            ? string.Join(", ", paymentTypeNames)
            : null;
        
        return new DocumentWithItemsDto
        {
            DocumentNumber = document.Number,
            CustomerName = document.Customer?.Name,
            Date = document.Date,
            DueDate = document.DueDate,
            PaymentTypeName = paymentTypeName,
            DocumentTypeCode = document.DocumentType.Code,
            DocumentTypeName = document.DocumentType.Name,
            DocumentDiscount = document.Discount,
            DocumentDiscountType = document.DiscountType,
            Items = document.DocumentItems.Select(di => new DocumentItemDto
            {
                Id = di.Id,
                ProductCode = di.Product.Code ?? "",
                ProductName = di.Product.Name,
                UnitOfMeasure = di.Product.MeasurementUnit ?? "ute",
                Quantity = di.Quantity,
                PriceBeforeTax = di.PriceBeforeTax,
                Tax = (di.Price - di.PriceBeforeTax) ,
                TaxRate = di.DocumentItemTaxes.FirstOrDefault()?.Tax?.Rate ?? 0m,
                Price = di.Price ,
                TotalBeforeDiscount = di.Total ,
                Discount = di.Discount ,
                DiscountType = di.DiscountType,
                Total = di.TotalAfterDocumentDiscount 
            }).ToList()
        };
    }
}