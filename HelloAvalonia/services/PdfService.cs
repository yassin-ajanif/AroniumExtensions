using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using QuestPDF.Companion;
using AroniumFactures.ViewModels;
using AroniumFactures;
using Avalonia.Media.Imaging;
using Avalonia.Platform;


namespace AroniumFactures.Services;

public class PdfService : IPdfService
{
    private Document CreateInvoiceDocument(MainWindowViewModel viewModel)
    {
        QuestPDF.Settings.License = LicenseType.Community;
        
        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.MarginBottom(0, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(10));

                page.Content()
                    .Column(column =>
                    {
                        // ==================== HEADER ====================
                        column.Item().Row(row =>
                        {
                            // Left: Logo
                            row.RelativeItem().Column(col =>
                            {
                                if (viewModel.HasLogo && viewModel.LogoImage != null)
                                {
                                    try
                                    {
                                        // Try to get logo from file path first (simpler and more reliable)
                                        var logoPath = GetLogoPathFromViewModel(viewModel);
                                        if (!string.IsNullOrEmpty(logoPath) && File.Exists(logoPath))
                                        {
                                            var logoBytes = File.ReadAllBytes(logoPath);
                                            col.Item().Width(150).Height(120).Image(logoBytes);
                                        }
                                        else
                                        {
                                            // Fallback: try to convert bitmap
                                            var logoBytes = ConvertLogoToBytes(viewModel.LogoImage);
                                            if (logoBytes.Length > 0)
                                            {
                                                col.Item().Width(150).Height(120).Image(logoBytes);
                                            }
                                        }
                                    }
                                    catch
                                    {
                                        // If logo conversion fails, just skip it
                                    }
                                }
                            });

                            // Right: Company info
                            row.RelativeItem(2).Column(col =>
                            {
                                col.Item().Text(viewModel.CompanyName ?? "")
                                    .FontSize(20).Bold();

                                col.Item().PaddingTop(5)
                                    .Text(viewModel.DocumentTypeName ?? "BL / Facture")
                                    .FontSize(16);
                            });
                        });

                        column.Item().PaddingVertical(5);

                        // ==================== INVOICE / CLIENT INFO ====================
                        column.Item().Row(row =>
                        {
                            // Left: Invoice Info
                            row.RelativeItem()
                                .Background(Colors.Grey.Lighten4)
                                .Border(1).BorderColor(Colors.Grey.Lighten2)
                                .CornerRadius(10)
                                .BorderAlignmentInside()
                                .Padding(20)
                                .Column(col =>
                                {
                                    col.Item().Row(r =>
                                    {
                                        r.ConstantItem(80).Text("N°:").Bold();
                                        r.RelativeItem().Text(viewModel.InvoiceNumber ?? "");
                                    });

                                    col.Item().PaddingTop(4).Row(r =>
                                    {
                                        r.ConstantItem(80).Text("Du:").Bold();
                                        r.RelativeItem().Text(viewModel.InvoiceDateString ?? "");
                                    });

                                    if (viewModel.DueDate.HasValue)
                                    {
                                        col.Item().PaddingTop(4).Row(r =>
                                        {
                                            r.ConstantItem(80)
                                                .Text("Date d'échéance:")
                                                .Bold()
                                                .FontColor(Colors.Red.Medium);

                                            r.RelativeItem()
                                                .Text(viewModel.DueDate.Value.ToString("dd/MM/yyyy"))
                                                .FontColor(Colors.Red.Medium);
                                        });
                                    }

                                    col.Item().PaddingTop(4).Row(r =>
                                    {
                                        r.ConstantItem(80).Text("Payé par:").Bold();
                                        r.RelativeItem().Text(viewModel.PaymentType ?? "");
                                    });
                                });

                            row.Spacing(10);

                            // Right: Client Info
                            row.RelativeItem()
                                .Background(Colors.Grey.Lighten4)
                                .Border(1).BorderColor(Colors.Grey.Lighten2)
                                .CornerRadius(10)
                                .BorderAlignmentInside()
                                .Padding(20)
                                .Column(col =>
                                {
                                    col.Item().Row(r =>
                                    {
                                        r.ConstantItem(60).Text("Client:").Bold();
                                        r.RelativeItem().Text(viewModel.CustomerName ?? "");
                                    });

                                    col.Item().PaddingTop(4).Row(r =>
                                    {
                                        r.ConstantItem(60).Text("ICE:").Bold();
                                        r.RelativeItem().Text(viewModel.IceNumber ?? "");
                                    });
                                });
                        });

                        column.Item().PaddingVertical(5);

                        // ==================== ITEMS TABLE ====================
                        if (viewModel.Items.Any())
                        {
                            column.Item()
                                .Border(1).BorderColor(Colors.Grey.Lighten2)
                                .CornerRadius(10)
                                .BorderAlignmentInside()
                                .Table(table =>
                                {
                                    table.ColumnsDefinition(columns =>
                                    {
                                        columns.RelativeColumn(1);
                                        columns.RelativeColumn(3);
                                        columns.RelativeColumn(1);
                                        columns.RelativeColumn(1.2f);
                                        columns.RelativeColumn(1);
                                        columns.RelativeColumn(1);
                                        columns.RelativeColumn(1.3f);
                                    });

                                    table.Header(header =>
                                    {
                                        header.Cell().Element(CellStyle).Text("Réf.").Bold();
                                        header.Cell().Element(CellStyle).Text("Désignation").Bold();
                                        header.Cell().Element(CellStyle).Text("Qté").Bold();
                                        header.Cell().Element(CellStyle).Text("PV.TTC NET").Bold();
                                        header.Cell().Element(CellStyle).Text("TVA").Bold();
                                        header.Cell().Element(CellStyle).Text("Remise").Bold();
                                        header.Cell().Element(CellStyle).Text("Mnt TTC").Bold();
                                    });

                                    // Items
                                    int rowIndex = 0;
                                    foreach (var item in viewModel.Items)
                                    {
                                        var isEvenRow = rowIndex % 2 == 0;
                                        var rowColor = isEvenRow ? Colors.White : Colors.Grey.Lighten5;
                                        
                                        table.Cell().Element(c => CellStyleWithBackground(c, rowColor)).Text(item.Reference ?? "");
                                        table.Cell().Element(c => CellStyleWithBackground(c, rowColor)).Text(item.Designation ?? "");
                                        table.Cell().Element(c => CellStyleWithBackground(c, rowColor)).Text(item.Quantity.ToString("F2"));
                                        table.Cell().Element(c => CellStyleWithBackground(c, rowColor)).Text(item.UnitPrice.ToString("F2"));
                                        table.Cell().Element(c => CellStyleWithBackground(c, rowColor)).Text(item.TvaRate.ToString("F2"));
                                        table.Cell().Element(c => CellStyleWithBackground(c, rowColor)).Text(item.ItemDiscountDisplay ?? "0.00");
                                        table.Cell().Element(c => CellStyleWithBackground(c, rowColor)).Text(item.TotalAfterDiscount.ToString("F2"));
                                        
                                        rowIndex++;
                                    }

                                    // Summary row
                                    var totalQuantity = viewModel.Items.Sum(item => item.Quantity);
                                    var numberOfReferences = viewModel.Items.Count;
                                    var summaryColor = Colors.Grey.Lighten4;
                                    
                                    table.Cell().ColumnSpan(2).Element(c => CellStyleWithBackground(c, summaryColor))
                                        .Text($"Total: {numberOfReferences} référence(s)").Bold();
                                    table.Cell().Element(c => CellStyleWithBackground(c, summaryColor))
                                        .Text(totalQuantity.ToString("F2")).Bold();
                                    // Empty cells for remaining columns
                                    table.Cell().Element(c => CellStyleWithBackground(c, summaryColor));
                                    table.Cell().Element(c => CellStyleWithBackground(c, summaryColor));
                                    table.Cell().Element(c => CellStyleWithBackground(c, summaryColor));
                                    table.Cell().Element(c => CellStyleWithBackground(c, summaryColor));
                                });
                        }

                        column.Item().PaddingVertical(20);

                        // ==================== TOTALS ====================
                        column.Item().ShowEntire().PaddingBottom(0).Row(row =>
                        {
                            // Amount in words
                            row.RelativeItem().Column(col =>
                            {
                                if (!string.IsNullOrEmpty(viewModel.TotalInWords))
                                {
                                    col.Item().PaddingBottom(5)
                                        .Text("Arrêté la présente facture à la somme de:")
                                        .FontSize(10);

                                    col.Item()
                                        .Background(Colors.Yellow.Lighten4)
                                        .CornerRadius(8)
                                        .Padding(10)
                                        .Text(viewModel.TotalInWords)
                                        .Bold();
                                }
                            });

                            row.Spacing(10);

                            // Totals box
                            row.RelativeItem()
                                .Background(Colors.Grey.Lighten4)
                                .Border(2).BorderColor(Colors.Grey.Lighten2)
                                .CornerRadius(10)
                                .BorderAlignmentInside()
                                .Padding(25)
                                .Column(col =>
                                {
                                    col.Item().Row(r =>
                                    {
                                        r.RelativeItem().Text("Total HT:").Bold();
                                        r.ConstantItem(100).AlignRight().Text($"{viewModel.TotalHT:F2} DH");
                                    });

                                    col.Item().PaddingTop(8).Row(r =>
                                    {
                                        r.RelativeItem().Text("Taxe:").Bold();
                                        r.ConstantItem(100).AlignRight().Text($"{viewModel.TotalTaxe:F2} DH");
                                    });

                                    col.Item().PaddingTop(16)
                                        .BorderTop(1).BorderColor(Colors.Grey.Lighten2)
                                        .PaddingTop(8)
                                        .Row(r =>
                                        {
                                            r.RelativeItem().Text("Total TTC:").Bold().FontSize(14);
                                            r.ConstantItem(100).AlignRight()
                                                .Text($"{viewModel.TotalTTC:F2} DH")
                                                .Bold()
                                                .FontSize(14)
                                                .FontColor(Colors.Blue.Medium);
                                        });
                                });
                        });
                    });

                page.Footer()
                    .AlignCenter()
                    .PaddingVertical(10)
                    .Column(col =>
                    {
                        var address = viewModel.CompanyAddress ?? "";
                        if (!string.IsNullOrEmpty(address))
                        {
                            // Split address by newlines and display each line
                            var addressLines = address.Split('\n', StringSplitOptions.RemoveEmptyEntries);
                            foreach (var line in addressLines)
                            {
                                col.Item().AlignCenter().Text(line.Trim());
                            }
                        }
                        
                        // Pagination
                        col.Item().PaddingTop(5).AlignCenter()
                            .Text(text =>
                            {
                                text.Span("Page ");
                                text.CurrentPageNumber();
                                text.Span(" / ");
                                text.TotalPages();
                            });
                    });
            });
        });
    }




    // --------- Local style helpers (keep them inside the method or move to private static methods) ---------

    static IContainer TableHeaderCell(IContainer container, Color borderColor, bool drawRightBorder)
    {
        container = container
            .Background(Colors.Grey.Lighten4)
            .PaddingVertical(6)
            .PaddingHorizontal(8)
            .BorderBottom(1).BorderColor(borderColor);

        if (drawRightBorder)
            container = container.BorderRight(1).BorderColor(borderColor);

        return container;
    }

    static IContainer TableBodyCell(IContainer container, Color borderColor, bool isLastRow, bool drawRightBorder)
    {
        container = container
            .PaddingVertical(5)
            .PaddingHorizontal(8);

        if (!isLastRow)
            container = container.BorderBottom(1).BorderColor(borderColor);

        if (drawRightBorder)
            container = container.BorderRight(1).BorderColor(borderColor);

        return container;
    }




    public async Task<string> GenerateInvoicePdfAsync(MainWindowViewModel viewModel, string outputPath)
    {
        // Run on background thread to avoid blocking
        return await Task.Run(() =>
        {
            try
            {
                var document = CreateInvoiceDocument(viewModel);
                document.GeneratePdf(outputPath);
                return outputPath;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de la génération du PDF: {ex.Message}", ex);
            }
        });
    }

    public async Task ShowInvoicePreviewAsync(MainWindowViewModel viewModel)
    {
        await Task.Run(() =>
        {
            try
            {
                var document = CreateInvoiceDocument(viewModel);
                
                // Try to show in Companion App
                try
                {
                    document.ShowInCompanion();
                }
                catch (Exception companionEx)
                {
                    // If Companion App is not available, fallback to generating a temporary PDF and opening it
                    var tempPdfPath = Path.Combine(Path.GetTempPath(), $"InvoicePreview_{Guid.NewGuid()}.pdf");
                    
                    try
                    {
                        document.GeneratePdf(tempPdfPath);
                      
                        
                        // Open the PDF with the default system viewer
                        var processStartInfo = new System.Diagnostics.ProcessStartInfo
                        {
                            FileName = tempPdfPath,
                            UseShellExecute = true
                        };
                        System.Diagnostics.Process.Start(processStartInfo);
                        
                        // Clean up the temp file after a delay (give time for PDF viewer to open it)
                        Task.Delay(5000).ContinueWith(_ =>
                        {
                            try
                            {
                                if (File.Exists(tempPdfPath))
                                {
                                    File.Delete(tempPdfPath);
                                }
                            }
                            catch
                            {
                                // Ignore cleanup errors
                            }
                        });
                    }
                    catch (Exception pdfEx)
                    {
                        throw new Exception(
                            $"Impossible de se connecter à QuestPDF Companion App et impossible de générer un PDF temporaire. " +
                            $"Veuillez installer et ouvrir QuestPDF Companion depuis https://www.questpdf.com/companion/ " +
                            $"ou utilisez le bouton 'Exporter PDF' pour générer un fichier PDF. " +
                            $"Erreur Companion: {companionEx.Message}. Erreur PDF: {pdfEx.Message}", companionEx);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de l'aperçu: {ex.Message}", ex);
            }
        });
    }

    private byte[] ConvertLogoToBytes(Bitmap bitmap)
    {
        // Convert Avalonia Bitmap to PNG bytes
        // Use a temporary file approach since direct SkiaSharp access is complex
        try
        {
            var tempFile = Path.Combine(Path.GetTempPath(), $"logo_{Guid.NewGuid()}.png");
            
            // Save bitmap to temporary file
            // Note: Avalonia Bitmap doesn't have a direct Save method in all versions
            // We'll use SkiaSharp through reflection if available
            using var memoryStream = new MemoryStream();
            
            // Try to get SkiaSharp image from bitmap using reflection
            var platformImplType = bitmap.GetType().GetProperty("PlatformImpl")?.GetValue(bitmap)?.GetType();
            if (platformImplType != null)
            {
                var skImageProperty = platformImplType.GetProperty("SkImage") 
                    ?? platformImplType.GetProperty("Image")
                    ?? platformImplType.GetProperty("Impl");
                
                if (skImageProperty != null)
                {
                    var platformImpl = bitmap.GetType().GetProperty("PlatformImpl")?.GetValue(bitmap);
                    if (platformImpl != null)
                    {
                        var skImage = skImageProperty.GetValue(platformImpl);
                        if (skImage != null)
                        {
                            // Use SkiaSharp SKImage.Encode method
                            var encodeMethod = skImage.GetType().GetMethod("Encode", new[] { typeof(SkiaSharp.SKEncodedImageFormat), typeof(int) });
                            if (encodeMethod != null)
                            {
                                var data = encodeMethod.Invoke(skImage, new object[] { SkiaSharp.SKEncodedImageFormat.Png, 100 });
                                if (data != null)
                                {
                                    var toArrayMethod = data.GetType().GetMethod("ToArray");
                                    if (toArrayMethod != null)
                                    {
                                        var result = toArrayMethod.Invoke(data, null) as byte[];
                                        if (result != null)
                                            return result;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        catch
        {
            // If conversion fails, return empty array
        }
        
        return Array.Empty<byte>();
    }

    private string? GetLogoPathFromViewModel(MainWindowViewModel viewModel)
    {
        // Try to get logo path from local settings
        try
        {
            var settingsService = ServiceProvider.LocalSettingsService;
            var settings = settingsService.LoadSettingsAsync().GetAwaiter().GetResult();
            return settings?.LogoPath ?? null;
        }
        catch
        {
            return null;
        }
    }

    private IContainer CellStyle(IContainer container)
    {
        return container
            .Border(1)
            .BorderColor(Colors.Grey.Lighten2)
            .Padding(5)
            .AlignMiddle();
    }

    private IContainer CellStyleWithBackground(IContainer container, Color backgroundColor)
    {
        return container
            .Background(backgroundColor)
            .Border(1)
            .BorderColor(Colors.Grey.Lighten2)
            .Padding(5)
            .AlignMiddle();
    }
}
