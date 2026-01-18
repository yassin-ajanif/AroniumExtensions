using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using AroniumFactures.Models;
using AroniumFactures;
using AroniumFactures.Services;

namespace AroniumFactures.ViewModels;

public class BonDeCommandeViewModel : ViewModelBase
{
    private MainWindowViewModel? _mainViewModel;

    // Client Information
    private string _clientName = string.Empty;
    private string _clientAddress = string.Empty;
    private string _clientSite = string.Empty;
    private string _clientIce = string.Empty;

    // Order Information
    private string _orderNumber = string.Empty;
    private DateTimeOffset _orderDate = DateTimeOffset.Now;
    private DateTimeOffset? _orderDateInput;
    private string _deliveryNoteNumber = string.Empty;
    private string _code = string.Empty;
    private string _quotationNumber = string.Empty;

    // Company Information
    private string _companyInfo = @"ENERGIE TECH SOLUTION SARL
SIEGE SOCIAL:
96, BOULEVARD ANFA, ÉTAGE 7, BUREAU 71, CASABLANCA.
RC N°: 646551 IF 66105741
PATENTE: 35407894 CNSS: 5740305 ICE 003597362000042 GSM:0654 351249
Gmail: energietechsolution@gmail.com";

    // Totals
    private decimal _totalHT = 0m;
    private decimal _vatAmount = 0m;
    private decimal _totalTTC = 0m;
    private decimal _vatRate = 20m;

    // Document Search
    private string _searchDocumentNumber = string.Empty;
    private bool _isLoadingDocument;
    private string _loadDocumentStatus = string.Empty;

    // Printing
    private bool _isPrinting;

    public ObservableCollection<InvoiceItem> Items { get; } = new();

    public RelayCommand LoadDocumentCommand { get; }
    public RelayCommand PrintBonDeCommandeCommand { get; }

    public BonDeCommandeViewModel(MainWindowViewModel? mainViewModel = null)
    {
        _mainViewModel = mainViewModel;
        LoadDocumentCommand = new RelayCommand(async () => await LoadDocumentFromDatabaseAsync(), () => !string.IsNullOrWhiteSpace(SearchDocumentNumber) && !IsLoadingDocument);
        PrintBonDeCommandeCommand = new RelayCommand(async () => await PrintBonDeCommandeAsync(), () => !IsPrinting);
        
        // Subscribe to item changes
        Items.CollectionChanged += (s, e) =>
        {
            if (e.NewItems != null)
            {
                foreach (InvoiceItem item in e.NewItems)
                {
                    item.PropertyChanged += (sender, args) => RecalculateTotals();
                }
            }
            RecalculateTotals();
        };
    }

    private void RecalculateTotals()
    {
        decimal totalHT = 0m;
        
        foreach (var item in Items)
        {
            totalHT += item.TotalHT;
        }

        TotalHT = totalHT;
        VatAmount = Math.Round(TotalHT * (VatRate / 100m), 2);
        TotalTTC = TotalHT + VatAmount;
    }

    // Client Properties
    public string ClientName
    {
        get => _clientName;
        set
        {
            if (_clientName != value)
            {
                _clientName = value;
                RaisePropertyChanged();
            }
        }
    }

    public string ClientAddress
    {
        get => _clientAddress;
        set
        {
            if (_clientAddress != value)
            {
                _clientAddress = value;
                RaisePropertyChanged();
            }
        }
    }

    public string ClientSite
    {
        get => _clientSite;
        set
        {
            if (_clientSite != value)
            {
                _clientSite = value;
                RaisePropertyChanged();
            }
        }
    }

    public string ClientIce
    {
        get => _clientIce;
        set
        {
            if (_clientIce != value)
            {
                _clientIce = value;
                RaisePropertyChanged();
            }
        }
    }

    // Order Properties
    public string OrderNumber
    {
        get => _orderNumber;
        set
        {
            if (_orderNumber != value)
            {
                _orderNumber = value;
                RaisePropertyChanged();
            }
        }
    }

    public DateTimeOffset OrderDate
    {
        get => _orderDate;
        set
        {
            if (_orderDate != value)
            {
                _orderDate = value;
                RaisePropertyChanged();
            }
        }
    }

    public DateTimeOffset? OrderDateInput
    {
        get => _orderDateInput;
        set
        {
            if (_orderDateInput != value)
            {
                _orderDateInput = value;
                RaisePropertyChanged();
            }
        }
    }

    public string DeliveryNoteNumber
    {
        get => _deliveryNoteNumber;
        set
        {
            if (_deliveryNoteNumber != value)
            {
                _deliveryNoteNumber = value;
                RaisePropertyChanged();
            }
        }
    }

    public string Code
    {
        get => _code;
        set
        {
            if (_code != value)
            {
                _code = value;
                RaisePropertyChanged();
            }
        }
    }

    public string QuotationNumber
    {
        get => _quotationNumber;
        set
        {
            if (_quotationNumber != value)
            {
                _quotationNumber = value;
                RaisePropertyChanged();
            }
        }
    }

    // Company Properties
    public string CompanyInfo
    {
        get => _companyInfo;
        set
        {
            if (_companyInfo != value)
            {
                _companyInfo = value;
                RaisePropertyChanged();
            }
        }
    }

    // Totals Properties
    public decimal TotalHT
    {
        get => _totalHT;
        private set
        {
            if (_totalHT != value)
            {
                _totalHT = value;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(TotalHTDisplay));
            }
        }
    }

    public string TotalHTDisplay => $"{TotalHT:N2}";

    public decimal VatRate
    {
        get => _vatRate;
        set
        {
            if (_vatRate != value)
            {
                _vatRate = value;
                RaisePropertyChanged();
                RecalculateTotals();
            }
        }
    }

    public decimal VatAmount
    {
        get => _vatAmount;
        private set
        {
            if (_vatAmount != value)
            {
                _vatAmount = value;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(VatAmountDisplay));
            }
        }
    }

    public string VatAmountDisplay => $"{VatAmount:N2}";

    public decimal TotalTTC
    {
        get => _totalTTC;
        private set
        {
            if (_totalTTC != value)
            {
                _totalTTC = value;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(TotalTTCDisplay));
            }
        }
    }

    public string TotalTTCDisplay => $"{TotalTTC:N2}";

    // Document Search Properties
    public string SearchDocumentNumber
    {
        get => _searchDocumentNumber;
        set
        {
            if (_searchDocumentNumber != value)
            {
                _searchDocumentNumber = value;
                RaisePropertyChanged();
                LoadDocumentCommand?.RaiseCanExecuteChanged();
            }
        }
    }

    public bool IsLoadingDocument
    {
        get => _isLoadingDocument;
        set
        {
            if (_isLoadingDocument != value)
            {
                _isLoadingDocument = value;
                RaisePropertyChanged();
                LoadDocumentCommand?.RaiseCanExecuteChanged();
            }
        }
    }

    public string LoadDocumentStatus
    {
        get => _loadDocumentStatus;
        set
        {
            if (_loadDocumentStatus != value)
            {
                _loadDocumentStatus = value;
                RaisePropertyChanged();
            }
        }
    }

    // Printing Property
    public bool IsPrinting
    {
        get => _isPrinting;
        set
        {
            if (_isPrinting != value)
            {
                _isPrinting = value;
                RaisePropertyChanged();
                PrintBonDeCommandeCommand?.RaiseCanExecuteChanged();
            }
        }
    }

    // Load Document Method
    private async Task LoadDocumentFromDatabaseAsync()
    {
        try
        {
            IsLoadingDocument = true;
            LoadDocumentStatus = "Chargement...";
            
            var invoiceService = AroniumFactures.ServiceProvider.InvoiceService;
            var documentData = await invoiceService.GetInvoiceWithItemsByNumberAsync(SearchDocumentNumber);
            
            if (documentData == null || documentData.Items.Count == 0)
            {
                LoadDocumentStatus = $"❌ Document '{SearchDocumentNumber}' introuvable ou vide";
                return;
            }
            
            // Set order information
            OrderNumber = documentData.DocumentNumber;
            OrderDate = new DateTimeOffset(documentData.Date);
            OrderDateInput = documentData.Date;
            
            // Set client information
            ClientName = documentData.CustomerName ?? string.Empty;
            
            // Clear existing items
            Items.Clear();
            
            // Convert DocumentItemDto to InvoiceItem
            foreach (var docItem in documentData.Items)
            {
                var invoiceItem = new InvoiceItem
                {
                    Reference = docItem.ProductCode,
                    Designation = docItem.ProductName,
                    Unit = docItem.UnitOfMeasure ?? "U",
                    Quantity = docItem.Quantity,
                    UnitPrice = docItem.PriceBeforeTax,
                    TaxAmount = docItem.Tax,
                    TvaRate = docItem.TaxRate,
                    ItemDiscount = docItem.Discount,
                    ItemDiscountType = docItem.DiscountType,
                    TotalTTC = docItem.TotalBeforeDiscount,
                    TotalAfterDiscount = docItem.Total
                };
                
                invoiceItem.PropertyChanged += (sender, args) => RecalculateTotals();
                Items.Add(invoiceItem);
            }
            
            RecalculateTotals();
            LoadDocumentStatus = $"✅ {documentData.Items.Count} produit(s) chargé(s)";
        }
        catch (Exception ex)
        {
            LoadDocumentStatus = $"❌ Erreur: {ex.Message}";
        }
        finally
        {
            IsLoadingDocument = false;
        }
    }

    // Print BonDeCommande Method
    private async Task PrintBonDeCommandeAsync()
    {
        if (_mainViewModel == null)
        {
            LoadDocumentStatus = "❌ Erreur: MainWindowViewModel non disponible";
            return;
        }

        try
        {
            IsPrinting = true;
            LoadDocumentStatus = "Génération du PDF...";
            
            await ServiceProvider.BonDeCommandePdfService.ShowBonDeCommandePreviewAsync(this, _mainViewModel);
            
            LoadDocumentStatus = "✅ PDF généré avec succès";
        }
        catch (Exception ex)
        {
            LoadDocumentStatus = $"❌ Erreur lors de la génération du PDF: {ex.Message}";
        }
        finally
        {
            IsPrinting = false;
        }
    }
}

