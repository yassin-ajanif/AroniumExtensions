using System.Threading.Tasks;
using AroniumFactures.Models;

namespace AroniumFactures.Repositories;

public interface IDocumentRepository
{
    /// <summary>
    /// Gets document with items and discount info by document number
    /// </summary>
    Task<DocumentWithItemsDto?> GetDocumentWithItemsByNumberAsync(string documentNumber);
}