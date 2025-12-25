using System.Collections.Generic;
using System.Threading.Tasks;
using HelloAvalonia.Models;

namespace HelloAvalonia.Repositories;

public interface IDocumentRepository
{
    /// <summary>
    /// Gets document with items and discount info by document number
    /// </summary>
    Task<DocumentWithItemsDto?> GetDocumentWithItemsByNumberAsync(string documentNumber);
}