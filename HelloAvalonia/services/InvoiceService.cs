using System.Collections.Generic;
using System.Threading.Tasks;
using AroniumFactures.Models;
using AroniumFactures.Repositories;

namespace AroniumFactures.Services;

public class InvoiceService : IInvoiceService
{
    private readonly IDocumentRepository _documentRepository;

    public InvoiceService(IDocumentRepository documentRepository)
    {
        _documentRepository = documentRepository;
    }

    public async Task<DocumentWithItemsDto?> GetInvoiceWithItemsByNumberAsync(string documentNumber)
    {
        return await _documentRepository.GetDocumentWithItemsByNumberAsync(documentNumber);
    }
}