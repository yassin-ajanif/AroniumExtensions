using System.Collections.Generic;
using System.Threading.Tasks;
using HelloAvalonia.Models;

namespace HelloAvalonia.Services;

public interface IInvoiceService
{
    Task<DocumentWithItemsDto?> GetInvoiceWithItemsByNumberAsync(string documentNumber);
}