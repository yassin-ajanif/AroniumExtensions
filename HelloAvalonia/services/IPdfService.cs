using System.Threading.Tasks;
using AroniumFactures.ViewModels;

namespace AroniumFactures.Services;

public interface IPdfService
{
    Task<string> GenerateInvoicePdfAsync(MainWindowViewModel viewModel, string outputPath);
    Task ShowInvoicePreviewAsync(MainWindowViewModel viewModel);
}

