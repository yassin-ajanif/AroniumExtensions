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

namespace AroniumFactures.Services;

public class BonDeCommandePdfService
{
    private Document CreateBonDeCommandeDocument(BonDeCommandeViewModel order, MainWindowViewModel main)
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
                                if (main.HasLogo && main.LogoImage != null)
                                {
                                    try
                                    {
                                        var logoPath = GetLogoPathFromMainViewModel(main);
                                        if (!string.IsNullOrEmpty(logoPath) && File.Exists(logoPath))
                                        {
                                            var logoBytes = File.ReadAllBytes(logoPath);
                                            col.Item().Width(150).Height(120).Image(logoBytes);
                                        }
                                        else
                                        {
                                            var logoBytes = ConvertLogoToBytes(main.LogoImage);
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

                            // Right: Company info and document title
                            row.RelativeItem(2).Column(col =>
                            {
                                // Extract company name from CompanyInfo or use mainViewModel
                                var companyName = ExtractCompanyName(order.CompanyInfo) ?? main.CompanyName ?? "";
                                col.Item().Text(companyName)
                                    .FontSize(20).Bold();

                                col.Item().PaddingTop(5)
                                    .Text("BON DE COMMANDE")
                                    .FontSize(16);
                            });
                        });

                        column.Item().PaddingVertical(5);

                        // ==================== DOCUMENT / CLIENT INFO ====================
                        column.Item().Row(row =>
                        {
                            // Left: Order Info
                            row.RelativeItem()
                                .Background(Colors.Grey.Lighten4)
                                .Border(1).BorderColor(Colors.Grey.Lighten2)
                                .CornerRadius(10)
                                .BorderAlignmentInside()
                                .Padding(20)
                                .Column(col =>
                                {
                                    if (!string.IsNullOrEmpty(order.OrderNumber))
                                    {
                                        col.Item().Row(r =>
                                        {
                                            r.ConstantItem(80).Text("N°:").Bold();
                                            r.RelativeItem().Text(order.OrderNumber);
                                        });
                                    }

                                    col.Item().PaddingTop(4).Row(r =>
                                    {
                                        r.ConstantItem(80).Text("Du:").Bold();
                                        r.RelativeItem().Text(order.OrderDate.ToString("dd/MM/yyyy"));
                                    });

                                    if (!string.IsNullOrEmpty(order.QuotationNumber))
                                    {
                                        col.Item().PaddingTop(4).Row(r =>
                                        {
                                            r.ConstantItem(80).Text("N° Devis:").Bold();
                                            r.RelativeItem().Text(order.QuotationNumber);
                                        });
                                    }
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
                                        r.RelativeItem().Text(order.ClientName ?? "");
                                    });

                                    if (!string.IsNullOrEmpty(order.ClientIce))
                                    {
                                        col.Item().PaddingTop(4).Row(r =>
                                        {
                                            r.ConstantItem(60).Text("ICE:").Bold();
                                            r.RelativeItem().Text(order.ClientIce);
                                        });
                                    }
                                });
                        });

                        column.Item().PaddingVertical(5);

                        // ==================== ITEMS TABLE ====================
                        if (order.Items.Any())
                        {
                            column.Item()
                                .Border(1).BorderColor(Colors.Grey.Lighten2)
                                .CornerRadius(10)
                                .BorderAlignmentInside()
                                .Table(table =>
                                {
                                    table.ColumnsDefinition(columns =>
                                    {
                                        columns.RelativeColumn(1);      // N°
                                        columns.RelativeColumn(3);      // DESIGNATION
                                        columns.RelativeColumn(1);      // UNITE
                                        columns.RelativeColumn(1);      // QTE
                                        columns.RelativeColumn(1.2f);   // PU HT
                                        columns.RelativeColumn(1.3f);   // PT HT
                                    });

                                    table.Header(header =>
                                    {
                                        header.Cell().Element(CellStyleHeader).Text("N°").Bold().FontColor(Colors.White);
                                        header.Cell().Element(CellStyleHeader).Text("DESIGNATION").Bold().FontColor(Colors.White);
                                        header.Cell().Element(CellStyleHeader).Text("UNITE").Bold().FontColor(Colors.White);
                                        header.Cell().Element(CellStyleHeader).Text("QTE").Bold().FontColor(Colors.White);
                                        header.Cell().Element(CellStyleHeader).Text("PU HT").Bold().FontColor(Colors.White);
                                        header.Cell().Element(CellStyleHeader).Text("PT HT").Bold().FontColor(Colors.White);
                                    });

                                    // Items
                                    int rowIndex = 0;
                                    foreach (var item in order.Items)
                                    {
                                        var isEvenRow = rowIndex % 2 == 0;
                                        var rowColor = isEvenRow ? Colors.White : Colors.Grey.Lighten5;
                                        
                                        table.Cell().Element(c => CellStyleWithBackground(c, rowColor)).Text(item.Reference ?? "");
                                        table.Cell().Element(c => CellStyleWithBackground(c, rowColor)).Text(item.Designation ?? "");
                                        table.Cell().Element(c => CellStyleWithBackground(c, rowColor)).Text(item.Unit ?? "U");
                                        table.Cell().Element(c => CellStyleWithBackground(c, rowColor)).Text(item.Quantity.ToString("F2"));
                                        table.Cell().Element(c => CellStyleWithBackground(c, rowColor)).Text(item.UnitPrice.ToString("F2"));
                                        table.Cell().Element(c => CellStyleWithBackground(c, rowColor)).Text(item.TotalHT.ToString("F2"));
                                        
                                        rowIndex++;
                                    }

                                    // Summary row
                                    var totalQuantity = order.Items.Sum(item => item.Quantity);
                                    var numberOfReferences = order.Items.Count;
                                    var summaryColor = Colors.Grey.Lighten4;
                                    
                                    table.Cell().ColumnSpan(2).Element(c => CellStyleWithBackground(c, summaryColor))
                                        .Text($"Total: {numberOfReferences} référence(s)").Bold();
                                    table.Cell().Element(c => CellStyleWithBackground(c, summaryColor));
                                    table.Cell().Element(c => CellStyleWithBackground(c, summaryColor))
                                        .Text(totalQuantity.ToString("F2")).Bold();
                                    // Empty cells for remaining columns
                                    table.Cell().Element(c => CellStyleWithBackground(c, summaryColor));
                                    table.Cell().Element(c => CellStyleWithBackground(c, summaryColor));
                                });
                        }

                        column.Item().PaddingVertical(10);

                        // ==================== TOTALS ====================
                        column.Item().ShowEntire().PaddingBottom(0).Row(row =>
                        {
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
                                        r.ConstantItem(100).AlignRight().Text($"{order.TotalHT:F2} DH");
                                    });

                                    col.Item().PaddingTop(8).Row(r =>
                                    {
                                        r.RelativeItem().Text($"TVA ({order.VatRate}%):").Bold();
                                        r.ConstantItem(100).AlignRight().Text($"{order.VatAmount:F2} DH");
                                    });

                                    col.Item().PaddingTop(16)
                                        .BorderTop(1).BorderColor(Colors.Grey.Lighten2)
                                        .PaddingTop(8)
                                        .Row(r =>
                                        {
                                            r.RelativeItem().Text("Total TTC:").Bold().FontSize(14);
                                            r.ConstantItem(100).AlignRight()
                                                .Text($"{order.TotalTTC:F2} DH")
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
                        // Extract company address from CompanyInfo or use mainViewModel
                        var companyAddress = ExtractCompanyAddress(order.CompanyInfo) ?? main.CompanyAddress ?? "";
                        if (!string.IsNullOrEmpty(companyAddress))
                        {
                            var addressLines = companyAddress.Split('\n', StringSplitOptions.RemoveEmptyEntries);
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

    private string? ExtractCompanyName(string companyInfo)
    {
        if (string.IsNullOrEmpty(companyInfo))
            return null;

        var lines = companyInfo.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        return lines.Length > 0 ? lines[0].Trim() : null;
    }

    private string? ExtractCompanyAddress(string companyInfo)
    {
        if (string.IsNullOrEmpty(companyInfo))
            return null;

        var lines = companyInfo.Split('\n', StringSplitOptions.RemoveEmptyEntries);
        if (lines.Length <= 1)
            return null;

        // Return all lines except the first (company name)
        return string.Join("\n", lines.Skip(1));
    }

    public async Task<string> GenerateBonDeCommandePdfAsync(BonDeCommandeViewModel order, MainWindowViewModel main, string outputPath)
    {
        return await Task.Run(() =>
        {
            try
            {
                var document = CreateBonDeCommandeDocument(order, main);
                document.GeneratePdf(outputPath);
                return outputPath;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de la génération du PDF: {ex.Message}", ex);
            }
        });
    }

    public async Task ShowBonDeCommandePreviewAsync(BonDeCommandeViewModel order, MainWindowViewModel main)
    {
        await Task.Run(() =>
        {
            try
            {
                var document = CreateBonDeCommandeDocument(order, main);
                
                // Try to show in Companion App
                try
                {
                    document.ShowInCompanion();
                }
                catch (Exception companionEx)
                {
                    // If Companion App is not available, fallback to generating a temporary PDF and opening it
                    var tempPdfPath = Path.Combine(Path.GetTempPath(), $"BonDeCommandePreview_{Guid.NewGuid()}.pdf");
                    
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
        try
        {
            using var memoryStream = new MemoryStream();
            
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

    private string? GetLogoPathFromMainViewModel(MainWindowViewModel main)
    {
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

    private IContainer CellStyleHeader(IContainer container)
    {
        return container
            .Background(Colors.Blue.Darken3)
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



