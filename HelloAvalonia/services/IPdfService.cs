using System.Threading.Tasks;
using HelloAvalonia.ViewModels;

namespace HelloAvalonia.Services;

public interface IPdfService
{
    Task<string> GenerateInvoicePdfAsync(MainWindowViewModel viewModel, string outputPath);
    Task ShowInvoicePreviewAsync(MainWindowViewModel viewModel);
}

