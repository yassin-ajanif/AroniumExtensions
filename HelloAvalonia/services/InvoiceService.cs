using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HelloAvalonia.Data.Entities;
using HelloAvalonia.Repositories;

namespace HelloAvalonia.Services;

public class InvoiceService : IInvoiceService
{
    private readonly IDocumentRepository _documentRepository;

    public InvoiceService(IDocumentRepository documentRepository)
    {
        _documentRepository = documentRepository;
    }

    public async Task<List<Document>> GetAllInvoicesAsync()
    {
        return await _documentRepository.GetAllAsync();
    }

    public async Task<Document?> GetInvoiceByIdAsync(int id)
    {
        return await _documentRepository.GetByIdAsync(id);
    }

    public async Task<Document?> GetInvoiceByNumberAsync(string number)
    {
        return await _documentRepository.GetByNumberAsync(number);
    }

    public async Task<List<Document>> GetInvoicesByCustomerAsync(int customerId)
    {
        return await _documentRepository.GetByCustomerIdAsync(customerId);
    }

    public async Task<Document> CreateInvoiceAsync(Document document)
    {
        // Set default values
        document.Date = DateTime.Now;
        document.StockDate = DateTime.Now;
        document.IsClockedOut = 0;
        document.PaidStatus = 0;
        document.DiscountType = 0;
        document.DiscountApplyRule = 0;
        document.ServiceType = 0;

        // Generate invoice number if not provided
        if (string.IsNullOrEmpty(document.Number))
        {
            document.Number = await GenerateInvoiceNumberAsync();
        }

        return await _documentRepository.CreateAsync(document);
    }

    public async Task UpdateInvoiceAsync(Document document)
    {
        await _documentRepository.UpdateAsync(document);
    }

    public async Task DeleteInvoiceAsync(int id)
    {
        await _documentRepository.DeleteAsync(id);
    }

    public async Task<string> GenerateInvoiceNumberAsync()
    {
        var nextNumber = await _documentRepository.GetNextDocumentNumberAsync();
        return $"INV-{DateTime.Now:yyyyMMdd}-{nextNumber:D4}";
    }
}










