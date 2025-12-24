using System.Collections.Generic;
using System.Threading.Tasks;
using HelloAvalonia.Data.Entities;

namespace HelloAvalonia.Repositories;

public interface IDocumentRepository
{
    Task<List<Document>> GetAllAsync();
    Task<Document?> GetByIdAsync(int id);
    Task<Document?> GetByNumberAsync(string number);
    Task<List<Document>> GetByCustomerIdAsync(int customerId);
    Task<Document> CreateAsync(Document document);
    Task UpdateAsync(Document document);
    Task DeleteAsync(int id);
    Task<int> GetNextDocumentNumberAsync();
}










