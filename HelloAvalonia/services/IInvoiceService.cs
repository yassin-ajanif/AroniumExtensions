using System.Collections.Generic;
using System.Threading.Tasks;
using HelloAvalonia.Data.Entities;

namespace HelloAvalonia.Services;

public interface IInvoiceService
{
    Task<List<Document>> GetAllInvoicesAsync();
    Task<Document?> GetInvoiceByIdAsync(int id);
    Task<Document?> GetInvoiceByNumberAsync(string number);
    Task<List<Document>> GetInvoicesByCustomerAsync(int customerId);
    Task<Document> CreateInvoiceAsync(Document document);
    Task UpdateInvoiceAsync(Document document);
    Task DeleteInvoiceAsync(int id);
    Task<string> GenerateInvoiceNumberAsync();
}










