using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HelloAvalonia.Data.Entities;
using HelloAvalonia.Services;

namespace HelloAvalonia.Examples;

/// <summary>
/// Examples of how to use the database services in your ViewModels
/// </summary>
public class DatabaseUsageExample
{
    private readonly IInvoiceService _invoiceService;
    private readonly ICustomerService _customerService;

    public DatabaseUsageExample()
    {
        // Get services from ServiceProvider
        _invoiceService = ServiceProvider.InvoiceService;
        _customerService = ServiceProvider.CustomerService;
    }

    /// <summary>
    /// Example: Load all invoices from database
    /// </summary>
    public async Task<List<Document>> LoadAllInvoicesAsync()
    {
        var invoices = await _invoiceService.GetAllInvoicesAsync();
        return invoices;
    }

    /// <summary>
    /// Example: Save a new invoice to database
    /// </summary>
    public async Task<Document> SaveInvoiceAsync(string customerName, List<DocumentItem> items)
    {
        // Create or get customer
        var customer = await _customerService.GetCustomerByNameAsync(customerName);
        if (customer == null)
        {
            customer = await _customerService.CreateCustomerAsync(new Customer
            {
                Name = customerName,
                IsCustomer = 1,
                IsEnabled = 1
            });
        }

        // Create invoice
        var invoice = new Document
        {
            CustomerId = customer.Id,
            Number = await _invoiceService.GenerateInvoiceNumberAsync(),
            Date = DateTime.Now,
            StockDate = DateTime.Now,
            UserId = 1, // Default user
            WarehouseId = 1, // Default warehouse
            DocumentTypeId = 1, // Invoice type
            Total = 0, // Will be calculated
            DocumentItems = items
        };

        // Calculate total
        foreach (var item in items)
        {
            invoice.Total += item.Total;
        }

        // Save to database
        var savedInvoice = await _invoiceService.CreateInvoiceAsync(invoice);
        return savedInvoice;
    }

    /// <summary>
    /// Example: Get invoice by number
    /// </summary>
    public async Task<Document?> GetInvoiceByNumberAsync(string invoiceNumber)
    {
        return await _invoiceService.GetInvoiceByNumberAsync(invoiceNumber);
    }

    /// <summary>
    /// Example: Get all customers
    /// </summary>
    public async Task<List<Customer>> LoadAllCustomersAsync()
    {
        return await _customerService.GetAllCustomersAsync();
    }

    /// <summary>
    /// Example: Delete an invoice
    /// </summary>
    public async Task DeleteInvoiceAsync(int invoiceId)
    {
        await _invoiceService.DeleteInvoiceAsync(invoiceId);
    }
}










