using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using AroniumFactures.Models;
using AroniumFactures.Helpers;

namespace AroniumFactures.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private string _invoiceNumber = string.Empty;
    private DateTimeOffset _invoiceDate = DateTimeOffset.Now;
    private string _paymentType = "C";
    private string _customerName = string.Empty;
    private string _iceNumber = "003721796000016";
    private string _companyName = "VOTRE ENTREPRISE SARL";
    private string _companyEmail = "contact@votreentreprise.ma";
    private string _companyAddress = "Angle Route Casa-Rabat et Av Mohamed Jamal Eddoorra -Ain Sebaa- Casablanca Maroc -Capital : 2 500 000 dhs\nTél. :0522 34 33 36 (LG) - Fax: 0522 34 33 03 - Email : contact@votreentreprise.ma\nR.C. : 168351 /CASA - Patente : 37965629 - I.F. : 40151021 - CNSS : 7892591 - ICE : 003721796000016";
    private Bitmap? _logoImage;
    private bool _isInitializing = true;
    private bool _initializationFailed;
    private string _initializationMessage = "Initialisation de la base de donnees...";
    private string _productCountDisplay = string.Empty;
    private string _productCountStatus = "Cliquer pour charger le nombre de produits";
    private string _searchDocumentNumber = string.Empty;
    private bool _isLoadingDocument;
    private string _loadDocumentStatus = string.Empty;
    private decimal _documentDiscount;
    private string _documentDiscountDisplay = "0.00";
    private DateTimeOffset? _dueDate;
    private string _documentTypeDisplay = string.Empty;
    private string _documentTypeName = "BL / Facture";
    private bool _hasDocumentType = false;

    public MainWindowViewModel()
    {
        AddItemCommand = new RelayCommand(AddItem);
        RemoveItemCommand = new RelayCommand<InvoiceItem>(RemoveItem);
        ChooseLogoCommand = new RelayCommand(ChooseLogo);
        RemoveLogoCommand = new RelayCommand(RemoveLogo);
        OpenSettingsCommand = new RelayCommand(OpenSettings);
        LoadProductCountCommand = new RelayCommand(async () => await LoadProductCountAsync(), () => IsInteractive);
        LoadDocumentCommand = new RelayCommand(async () => await LoadDocumentItemsAsync(), () => !string.IsNullOrWhiteSpace(SearchDocumentNumber) && !IsLoadingDocument);
        ApplyDiscountBeforeTaxCommand = new RelayCommand(() => ApplyDiscountBeforeTax = true);
        ApplyDiscountAfterTaxCommand = new RelayCommand(() => ApplyDiscountBeforeTax = false);
        ExportToPdfCommand = new RelayCommand(async () => await ExportToPdfAsync());
        PrintCommand = new RelayCommand(async () => await PrintAsync());
        PreviewInvoiceCommand = new RelayCommand(async () => await PreviewInvoiceAsync());

        ProductCountDisplay = _productCountStatus;
        
        // Add default demo items
        Items.Add(new InvoiceItem 
        { 
            Reference = "98958", 
            Designation = "PLATEAU À LAMELLES - 115 MM 22 23 MM, 40",
            Quantity = 5,
            UnitPrice = 39m,
            TvaRate = 20m
        });
        
        Items.Add(new InvoiceItem 
        { 
            Reference = "109549", 
            Designation = "DISQUE ABRSIF À TRONC. POUR INOX DIM115X1.2X22.2MM REF CADG-SGC12221/15",
            Quantity = 10,
            UnitPrice = 7m,
            TvaRate = 20m
        });
        
        Items.Add(new InvoiceItem 
        { 
            Reference = "125506", 
            Designation = "DISQUE STANDARD POUR METAL À MOYEU DÉPORTÉ 115X2.5X22.23MM",
            Quantity = 1,
            UnitPrice = 5.90m,
            TvaRate = 20m
        });
        
        Items.Add(new InvoiceItem 
        { 
            Reference = "109556", 
            Designation = "DISQUE ABRSIF À TRONC. POUR MÉTAL D 350X3.5X25.4MM REF CADG-MC3525350",
            Quantity = 5,
            UnitPrice = 40m,
            TvaRate = 20m
        });
        
        // Generate invoice number
        _invoiceNumber = $"{DateTime.Now:yyyyMMddHHmmss}";
        
        // Subscribe to item changes for totals calculation
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

    public RelayCommand AddItemCommand { get; }
    public RelayCommand<InvoiceItem> RemoveItemCommand { get; }
    public RelayCommand ChooseLogoCommand { get; }
    public RelayCommand RemoveLogoCommand { get; }
    public RelayCommand OpenSettingsCommand { get; }
    public RelayCommand LoadProductCountCommand { get; }
    public RelayCommand LoadDocumentCommand { get; }
    public RelayCommand ApplyDiscountBeforeTaxCommand { get; }
    public RelayCommand ApplyDiscountAfterTaxCommand { get; }
    public RelayCommand ExportToPdfCommand { get; }
    public RelayCommand PrintCommand { get; }
    public RelayCommand PreviewInvoiceCommand { get; }

    public bool IsInitializing
    {
        get => _isInitializing;
        private set
        {
            if (_isInitializing != value)
            {
                _isInitializing = value;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(IsInteractive));
                LoadProductCountCommand.RaiseCanExecuteChanged();
            }
        }
    }

    public bool InitializationFailed
    {
        get => _initializationFailed;
        private set
        {
            if (_initializationFailed != value)
            {
                _initializationFailed = value;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(IsInteractive));
                LoadProductCountCommand.RaiseCanExecuteChanged();
            }
        }
    }

    public string InitializationMessage
    {
        get => _initializationMessage;
        private set
        {
            if (_initializationMessage != value)
            {
                _initializationMessage = value;
                RaisePropertyChanged();
            }
        }
    }

    public bool IsInteractive => !_isInitializing;
    
    public ObservableCollection<InvoiceItem> Items { get; } = new();

    public string InvoiceNumber
    {
        get => _invoiceNumber;
        set
        {
            if (_invoiceNumber != value)
            {
                _invoiceNumber = value;
                RaisePropertyChanged();
            }
        }
    }

    public DateTimeOffset InvoiceDate
    {
        get => _invoiceDate;
        set
        {
            if (_invoiceDate != value)
            {
                _invoiceDate = value;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(InvoiceDateString));
            }
        }
    }

    public string InvoiceDateString
    {
        get => _invoiceDate.ToString("dd/MM/yyyy");
        set
        {
            if (DateTimeOffset.TryParseExact(value, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out var date))
            {
                InvoiceDate = date;
            }
        }
    }

    public string PaymentType
    {
        get => _paymentType;
        set
        {
            if (_paymentType != value)
            {
                _paymentType = value;
                RaisePropertyChanged();
            }
        }
    }

    public string CustomerName
    {
        get => _customerName;
        set
        {
            if (_customerName != value)
            {
                _customerName = value;
                RaisePropertyChanged();
            }
        }
    }

    public string IceNumber
    {
        get => _iceNumber;
        set
        {
            if (_iceNumber != value)
            {
                _iceNumber = value;
                RaisePropertyChanged();
                _ = SaveSettingsAsync();
            }
        }
    }

    public string CompanyName
    {
        get => _companyName;
        set
        {
            if (_companyName != value)
            {
                _companyName = value;
                RaisePropertyChanged();
                _ = SaveSettingsAsync();
            }
        }
    }

    public string CompanyAddress
    {
        get => _companyAddress;
        set
        {
            if (_companyAddress != value)
            {
                _companyAddress = value;
                RaisePropertyChanged();
                _ = SaveSettingsAsync();
            }
        }
    }

    public string CompanyEmail
    {
        get => _companyEmail;
        set
        {
            if (_companyEmail != value)
            {
                _companyEmail = value;
                RaisePropertyChanged();
            }
        }
    }

    public string ProductCountDisplay
    {
        get => _productCountDisplay;
        set
        {
            if (_productCountDisplay != value)
            {
                _productCountDisplay = value;
                RaisePropertyChanged();
            }
        }
    }

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

    public decimal DocumentDiscount
    {
        get => _documentDiscount;
        set
        {
            if (_documentDiscount != value)
            {
                _documentDiscount = value;
                RaisePropertyChanged();
                UpdateDocumentDiscountDisplay();
            }
        }
    }

    public string DocumentDiscountDisplay
    {
        get => _documentDiscountDisplay;
        private set
        {
            if (_documentDiscountDisplay != value)
            {
                _documentDiscountDisplay = value;
                RaisePropertyChanged();
            }
        }
    }

    private void UpdateDocumentDiscountDisplay()
    {
        DocumentDiscountDisplay = $"{DocumentDiscount:F2}";
    }

    public DateTimeOffset? DueDate
    {
        get => _dueDate;
        set
        {
            if (_dueDate != value)
            {
                _dueDate = value;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(DueDateString));
            }
        }
    }

    public string DueDateString
    {
        get => _dueDate?.ToString("dd/MM/yyyy") ?? string.Empty;
        set
        {
            if (DateTimeOffset.TryParseExact(value, "dd/MM/yyyy", null, System.Globalization.DateTimeStyles.None, out var date))
            {
                DueDate = date;
            }
        }
    }

    public string DocumentTypeDisplay
    {
        get => _documentTypeDisplay;
        set
        {
            if (_documentTypeDisplay != value)
            {
                _documentTypeDisplay = value;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(DocumentTypeName));
            }
        }
    }

    public string DocumentTypeName
    {
        get => _documentTypeName;
        set
        {
            if (_documentTypeName != value)
            {
                _documentTypeName = value;
                RaisePropertyChanged();
            }
        }
    }

    public bool HasDocumentType
    {
        get => _hasDocumentType;
        set
        {
            if (_hasDocumentType != value)
            {
                _hasDocumentType = value;
                RaisePropertyChanged();
            }
        }
    }

    public Bitmap? LogoImage
    {
        get => _logoImage;
        set
        {
            if (_logoImage != value)
            {
                _logoImage = value;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(HasLogo));
            }
        }
    }

    public bool HasLogo => _logoImage != null;

    private decimal _totalHT;
    public decimal TotalHT
    {
        get => _totalHT;
        private set
        {
            if (_totalHT != value)
            {
                _totalHT = value;
                RaisePropertyChanged();
            }
        }
    }

    private decimal _totalTaxe;
    public decimal TotalTaxe
    {
        get => _totalTaxe;
        private set
        {
            if (_totalTaxe != value)
            {
                _totalTaxe = value;
                RaisePropertyChanged();
            }
        }
    }

    private decimal _totalTTC;
    public decimal TotalTTC
    {
        get => _totalTTC;
        private set
        {
            if (_totalTTC != value)
            {
                _totalTTC = value;
                RaisePropertyChanged();
                RaisePropertyChanged(nameof(TotalInWords));
            }
        }
    }

    // 1) Add this property in your ViewModel (default = BeforeTax)
private bool _applyDiscountBeforeTax = true;
public bool ApplyDiscountBeforeTax
{
    get => _applyDiscountBeforeTax;
    set
    {
        if (_applyDiscountBeforeTax != value)
        {
            _applyDiscountBeforeTax = value;
            RaisePropertyChanged();
            RaisePropertyChanged(nameof(BeforeTaxButtonBackground));
            RaisePropertyChanged(nameof(AfterTaxButtonBackground));
            RecalculateTotals();
        }
    }
}

// Button background colors based on active mode
public string BeforeTaxButtonBackground => ApplyDiscountBeforeTax ? "#10B981" : "#D1D5DB";
public string AfterTaxButtonBackground => !ApplyDiscountBeforeTax ? "#10B981" : "#D1D5DB";

    public string TotalInWords => NumberToFrenchWords.ConvertToWords(TotalTTC);

    
    private void AddItem()
    {
        Items.Add(new InvoiceItem 
        { 
            Reference = (Items.Count + 1).ToString("D3"),
            Quantity = 1,
            TvaRate = 20m
        });
    }

    private void RemoveItem(InvoiceItem? item)
    {
        if (item != null && Items.Count > 1)
        {
            Items.Remove(item);
        }
    }

    private static decimal GetItemFinalTtc(InvoiceItem i)
    {
        // Prefer persisted totals when loading from the database; otherwise derive from current inputs
        if (i.TotalAfterDiscount > 0)
            return i.TotalAfterDiscount;

        var baseTtc = i.UnitPrice * i.Quantity;
        var discount = i.ItemDiscountType == 0
            ? baseTtc * (i.ItemDiscount / 100m)
            : i.ItemDiscount;

        return Math.Max(baseTtc - discount, 0);
    }

 // 2) Parent function decides which calculation to use
private void RecalculateTotals()
{
    if (ApplyDiscountBeforeTax)
        RecalculateBeforeTax();
    else
        RecalculateAfterTax();
}

// 3) Remise AVANT TVA (discount reduces taxable base)
private void RecalculateBeforeTax()
{
    decimal totalHT = 0m;
    decimal totalTax = 0m;
    decimal totalTTC = 0m;

    foreach (var i in Items)
    {
        decimal qty = (decimal)i.Quantity;
        decimal rate = i.TvaRate / 100m;

        // UnitPrice is HT
        decimal lineHTBeforeDiscount = i.UnitPrice * qty;

        // Discount in HT (percent or amount)
        decimal discountHT = i.ItemDiscountType == 0
            ? lineHTBeforeDiscount * (i.ItemDiscount / 100m)
            : i.ItemDiscount;

        // Guards
        if (discountHT < 0m) discountHT = 0m;
        if (discountHT > lineHTBeforeDiscount) discountHT = lineHTBeforeDiscount;

        decimal lineHT = lineHTBeforeDiscount - discountHT;
        if (lineHT < 0m) lineHT = 0m;

        decimal lineTax = Math.Round(lineHT * rate, 2, MidpointRounding.AwayFromZero);
        decimal lineTTC = Math.Round(lineHT + lineTax, 2, MidpointRounding.AwayFromZero);

        totalHT += Math.Round(lineHT, 2, MidpointRounding.AwayFromZero);
        totalTax += lineTax;
        totalTTC += lineTTC;
    }

    TotalHT = totalHT;
    TotalTaxe = totalTax;
    TotalTTC = totalTTC;
}

// 4) Remise APRÈS TVA (discount does NOT reduce tax)
private void RecalculateAfterTax()
{
    decimal totalHT = 0m;
    decimal totalTax = 0m;
    decimal totalTTC = 0m;

    foreach (var i in Items)
    {
        decimal qty = (decimal)i.Quantity;
        decimal rate = i.TvaRate / 100m;

        // HT line total
        decimal lineHTBeforeDiscount = i.UnitPrice * qty;
        if (lineHTBeforeDiscount < 0m) lineHTBeforeDiscount = 0m;

        // TTC BEFORE discount (compute from HT)
        // Use line-level rounding to match typical POS behavior.
        decimal lineTTCBeforeDiscount = Math.Round(lineHTBeforeDiscount * (1m + rate), 2, MidpointRounding.AwayFromZero);

        // Discount applied on TTC (percent or amount)
        decimal discountTTC = 0m;

        if (i.ItemDiscountType == 0) // percent
        {
            discountTTC = lineTTCBeforeDiscount * (i.ItemDiscount / 100m);
        }
        else // amount (in TTC)
        {
            discountTTC = i.ItemDiscount;
        }

        // Guards
        if (discountTTC < 0m) discountTTC = 0m;
        if (discountTTC > lineTTCBeforeDiscount) discountTTC = lineTTCBeforeDiscount;

        // TTC AFTER discount
        decimal lineTTCAfterDiscount = Math.Round(lineTTCBeforeDiscount - discountTTC, 2, MidpointRounding.AwayFromZero);

        // Recompute HT + TAX from discounted TTC (this is the missing part)
        decimal lineHTAfterDiscount = (rate == 0m)
            ? lineTTCAfterDiscount
            : Math.Round(lineTTCAfterDiscount / (1m + rate), 2, MidpointRounding.AwayFromZero);

        decimal lineTaxAfterDiscount = Math.Round(lineTTCAfterDiscount - lineHTAfterDiscount, 2, MidpointRounding.AwayFromZero);

        // Accumulate
        totalHT += lineHTAfterDiscount;
        totalTax += lineTaxAfterDiscount;
        totalTTC += lineTTCAfterDiscount;
    }

    TotalHT = totalHT;
    TotalTaxe = totalTax;
    TotalTTC = totalTTC;
}




    public async Task InitializeAsync(string mainDbPath)
    {
        InitializationFailed = false;
        IsInitializing = true;
        InitializationMessage = "Initialisation de la base de donnees...";

        try
        {
            var dbInitialized = await DatabaseInitializer.InitializeDatabaseAsync(mainDbPath);
            if (!dbInitialized)
            {
                InitializationMessage = "Echec de l'initialisation de la base de donnees.";
                InitializationFailed = true;
                return;
            }

            ServiceProvider.Initialize(mainDbPath);
            ProductCountDisplay = _productCountStatus;
            InitializationMessage = "Base de donnees prete.";
            
            // Load saved settings after ServiceProvider is initialized
            await LoadSavedSettingsAsync();
        }
        catch (Exception ex)
        {
            InitializationMessage = $"Erreur d'initialisation: {ex.Message}";
            InitializationFailed = true;
        }
        finally
        {
            IsInitializing = false;
        }
    }

    private async void ChooseLogo()
    {
        var window = (Avalonia.Application.Current?.ApplicationLifetime as Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime)?.MainWindow;
        if (window == null) return;

        var dialog = new Avalonia.Platform.Storage.FilePickerOpenOptions
        {
            Title = "Choisir un logo",
            AllowMultiple = false,
            FileTypeFilter = new[]
            {
                new Avalonia.Platform.Storage.FilePickerFileType("Images")
                {
                    Patterns = new[] { "*.png", "*.jpg", "*.jpeg", "*.bmp", "*.gif" }
                }
            }
        };

        var result = await window.StorageProvider.OpenFilePickerAsync(dialog);
        if (result.Count == 0) return;

        var filePath = result[0].Path.LocalPath;
        
        try
        {
            // Create logos folder in app data
            var logoFolder = System.IO.Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "AroniumFactures",
                "Logos"
            );
            Directory.CreateDirectory(logoFolder);
            
            // Copy image to local folder with extension
            var extension = System.IO.Path.GetExtension(filePath);
            var savedLogoPath = System.IO.Path.Combine(logoFolder, $"company_logo{extension}");
            
            // Copy file
            System.IO.File.Copy(filePath, savedLogoPath, overwrite: true);
            
            // Load the logo
            LogoImage = new Bitmap(savedLogoPath);
            
            // Save path to settings
            var settingsService = ServiceProvider.LocalSettingsService;
            var settings = await settingsService.LoadSettingsAsync();
            settings.LogoPath = savedLogoPath;
            await settingsService.SaveSettingsAsync(settings);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving logo: {ex.Message}");
            LogoImage = null;
        }
    }

    private async void RemoveLogo()
    {
        LogoImage = null;
        
        // Clear logo from settings
        try
        {
            var settingsService = ServiceProvider.LocalSettingsService;
            var settings = await settingsService.LoadSettingsAsync();
            settings.LogoPath = null;
            await settingsService.SaveSettingsAsync(settings);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error removing logo: {ex.Message}");
        }
    }

    private async Task LoadSavedSettingsAsync()
    {
        try
        {
            var settingsService = ServiceProvider.LocalSettingsService;
            var settings = await settingsService.LoadSettingsAsync();
            
            // Load logo
            if (!string.IsNullOrEmpty(settings.LogoPath) && System.IO.File.Exists(settings.LogoPath))
            {
                LogoImage = new Bitmap(settings.LogoPath);
            }
            
            // Load company name
            if (!string.IsNullOrEmpty(settings.CompanyName))
            {
                _companyName = settings.CompanyName;
                RaisePropertyChanged(nameof(CompanyName));
            }
            
            // Load ICE number
            if (!string.IsNullOrEmpty(settings.IceNumber))
            {
                _iceNumber = settings.IceNumber;
                RaisePropertyChanged(nameof(IceNumber));
            }
            
            // Load company info (footer text)
            if (!string.IsNullOrEmpty(settings.CompanyInfo))
            {
                _companyAddress = settings.CompanyInfo;
                RaisePropertyChanged(nameof(CompanyAddress));
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading saved settings: {ex.Message}");
        }
    }

    private async Task SaveSettingsAsync()
    {
        try
        {
            var settingsService = ServiceProvider.LocalSettingsService;
            var settings = await settingsService.LoadSettingsAsync();
            
            // Update only the changed fields, keep LogoPath if it exists
            settings.CompanyName = _companyName;
            settings.IceNumber = _iceNumber;
            settings.CompanyInfo = _companyAddress;
            
            await settingsService.SaveSettingsAsync(settings);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving settings: {ex.Message}");
        }
    }

    private async Task LoadProductCountAsync()
    {
        try
        {
            var productService = ServiceProvider.ProductService;
            ProductCountDisplay = "Chargement...";
            var count = await productService.GetProductCountAsync();

            ProductCountDisplay = $"{count} produits";
            Console.WriteLine($"Total products: {count}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading product count: {ex.Message}");
            ProductCountDisplay = "Erreur lors du chargement";
        }
    }

    private async void OpenSettings()
    {
        var window = (Avalonia.Application.Current?.ApplicationLifetime as Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime)?.MainWindow;
        if (window == null) return;

        var settingsDialog = new SettingsDialog();
        var result = await settingsDialog.ShowDialog<bool>(window);
        
        if (result)
        {
            // Settings saved, offer to reload database
            // TODO: Add reload functionality
            Console.WriteLine("Settings saved! Restart app to use new database.");
        }
    }

    private async Task ExportToPdfAsync()
    {
        try
        {
            var window = (Avalonia.Application.Current?.ApplicationLifetime as Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime)?.MainWindow;
            if (window == null)
            {
                LoadDocumentStatus = "Erreur: Fenêtre principale introuvable";
                return;
            }

            var defaultFileName = $"Facture_{InvoiceNumber}_{DateTime.Now:yyyyMMdd}.pdf";
            
            var file = await window.StorageProvider.SaveFilePickerAsync(new Avalonia.Platform.Storage.FilePickerSaveOptions
            {
                Title = "Enregistrer la facture en PDF",
                DefaultExtension = "pdf",
                SuggestedFileName = defaultFileName,
                FileTypeChoices = new[]
                {
                    new Avalonia.Platform.Storage.FilePickerFileType("Fichier PDF")
                    {
                        Patterns = new[] { "*.pdf" }
                    }
                }
            });

            if (file != null)
            {
                LoadDocumentStatus = "Génération du PDF...";
                
                var filePath = file.Path.LocalPath;
                
                // Run PDF generation on background thread to avoid blocking UI
                await Task.Run(async () =>
                {
                    try
                    {
                        await ServiceProvider.PdfService.GenerateInvoicePdfAsync(this, filePath);
                        
                        // Update UI on main thread
                        await Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
                        {
                            LoadDocumentStatus = $"✅ PDF généré avec succès: {Path.GetFileName(filePath)}";
                        });
                    }
                    catch (Exception ex)
                    {
                        // Update UI on main thread with error
                        await Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
                        {
                            LoadDocumentStatus = $"❌ Erreur lors de la génération du PDF: {ex.Message}";
                        });
                    }
                });
            }
        }
        catch (Exception ex)
        {
            LoadDocumentStatus = $"❌ Erreur lors de la génération du PDF: {ex.Message}";
        }
    }

    private async Task PrintAsync()
    {
        try
        {
            var window = (Avalonia.Application.Current?.ApplicationLifetime as Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime)?.MainWindow;
            if (window == null)
            {
                LoadDocumentStatus = "Erreur: Fenêtre principale introuvable";
                return;
            }

            LoadDocumentStatus = "Préparation de l'impression...";
            
            // Generate PDF to a temporary file
            var tempPdfPath = Path.Combine(Path.GetTempPath(), $"Facture_{InvoiceNumber}_{DateTime.Now:yyyyMMddHHmmss}.pdf");
            
            await Task.Run(async () =>
            {
                try
                {
                    // Generate PDF
                    await ServiceProvider.PdfService.GenerateInvoicePdfAsync(this, tempPdfPath);
                    
                    // Print using Windows default printer
                    await Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
                    {
                        try
                        {
                            var processStartInfo = new ProcessStartInfo
                            {
                                FileName = tempPdfPath,
                                Verb = "print",
                                UseShellExecute = true,
                                CreateNoWindow = true
                            };
                            
                            Process.Start(processStartInfo);
                            
                            LoadDocumentStatus = "✅ Document envoyé à l'imprimante";
                            
                            // Clean up temp file after a delay (give time for print spooler)
                            Task.Delay(5000).ContinueWith(_ =>
                            {
                                try
                                {
                                    if (File.Exists(tempPdfPath))
                                        File.Delete(tempPdfPath);
                                }
                                catch { }
                            });
                        }
                        catch (Exception ex)
                        {
                            LoadDocumentStatus = $"Erreur d'impression: {ex.Message}";
                            
                            // Try to open PDF for manual printing
                            try
                            {
                                Process.Start(new ProcessStartInfo
                                {
                                    FileName = tempPdfPath,
                                    UseShellExecute = true
                                });
                                LoadDocumentStatus = "PDF ouvert - vous pouvez imprimer manuellement";
                            }
                            catch { }
                        }
                    });
                }
                catch (Exception ex)
                {
                    await Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
                    {
                        LoadDocumentStatus = $"Erreur lors de la génération du PDF: {ex.Message}";
                    });
                }
            });
        }
        catch (Exception ex)
        {
            LoadDocumentStatus = $"Erreur: {ex.Message}";
        }
    }

    private async Task PreviewInvoiceAsync()
    {
        try
        {
            LoadDocumentStatus = "Ouverture de l'aperçu...";
            
            await Task.Run(async () =>
            {
                try
                {
                    await ServiceProvider.PdfService.ShowInvoicePreviewAsync(this);
                    
                    await Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
                    {
                        LoadDocumentStatus = "✅ Aperçu ouvert dans QuestPDF Companion";
                    });
                }
                catch (Exception ex)
                {
                    await Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(() =>
                    {
                        LoadDocumentStatus = $"❌ Erreur: {ex.Message}. Assurez-vous que QuestPDF Companion est installé et ouvert.";
                    });
                }
            });
        }
        catch (Exception ex)
        {
            LoadDocumentStatus = $"❌ Erreur: {ex.Message}";
        }
    }

    private async Task LoadDocumentItemsAsync()
    {
        try
        {
            IsLoadingDocument = true;
            LoadDocumentStatus = "Chargement...";
            
            // Get invoice service
            var invoiceService = AroniumFactures.ServiceProvider.InvoiceService;
            
            // Load document with items and discount in ONE call
            var documentData = await invoiceService.GetInvoiceWithItemsByNumberAsync(SearchDocumentNumber);
            
            if (documentData == null || documentData.Items.Count == 0)
            {
                LoadDocumentStatus = $"❌ Document '{SearchDocumentNumber}' introuvable ou vide";
                return;
            }
            
            // Set invoice number
            InvoiceNumber = documentData.DocumentNumber;
            
            // Set invoice date
            InvoiceDate = new DateTimeOffset(documentData.Date);
            
            // Set payment type
            PaymentType = documentData.PaymentTypeName ?? string.Empty;
            
            // Set document type with French translation
            if (!string.IsNullOrEmpty(documentData.DocumentTypeCode) && 
                !string.IsNullOrEmpty(documentData.DocumentTypeName))
            {
                var translatedName = Helpers.DocumentTypeTranslator.Translate(documentData.DocumentTypeName);
                DocumentTypeDisplay = $"{documentData.DocumentTypeCode} - {translatedName}";
                DocumentTypeName = translatedName;
                HasDocumentType = true;
            }
            else
            {
                DocumentTypeDisplay = string.Empty;
                DocumentTypeName = "BL / Facture";
                HasDocumentType = false;
            }
            
            // Set due date
            if (documentData.DueDate.HasValue)
            {
                DueDate = new DateTimeOffset(documentData.DueDate.Value);
            }
            else
            {
                DueDate = null;
            }
            
            // Set customer name
            CustomerName = documentData.CustomerName ?? "N/A";
            
            // Set discount (convert from storage format)
            DocumentDiscount = documentData.DocumentDiscount ;
            
            // Clear existing items
            Items.Clear();
            
            // Convert DocumentItemDto to InvoiceItem - use database values directly (no calculations)
            foreach (var docItem in documentData.Items)
            {
                Items.Add(new InvoiceItem
                {
                    Reference = docItem.ProductCode,
                    Designation = docItem.ProductName,
                    Quantity = docItem.Quantity,
                    UnitPrice = docItem.PriceBeforeTax,
                    TaxAmount = docItem.Tax,
                    TvaRate = docItem.TaxRate,
                    ItemDiscount = docItem.Discount,
                    ItemDiscountType = docItem.DiscountType,
                    TotalTTC = docItem.TotalBeforeDiscount,
                    TotalAfterDiscount = docItem.Total
                });
            }
            
            LoadDocumentStatus = $"✅ {documentData.Items.Count} produit(s) chargé(s)";
        }
        catch (Exception ex)
        {
            LoadDocumentStatus = $"❌ Erreur: {ex.Message}";
            InvoiceNumber = string.Empty;
            InvoiceDate = DateTimeOffset.Now;
            PaymentType = string.Empty;
            DocumentTypeDisplay = string.Empty;
            DocumentTypeName = "BL / Facture";
            HasDocumentType = false;
            DueDate = null;
            CustomerName = string.Empty;
            DocumentDiscount = 0;
        }
        finally
        {
            IsLoadingDocument = false;
        }
    }
}
