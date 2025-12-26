using System.Collections.Generic;
using System.Threading.Tasks;
using AroniumFactures.Models;

namespace AroniumFactures.Services;

public interface IInvoiceService
{
    Task<DocumentWithItemsDto?> GetInvoiceWithItemsByNumberAsync(string documentNumber);
}