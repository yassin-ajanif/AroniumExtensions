using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using HelloAvalonia.Models;
using HelloAvalonia.Helpers;

namespace HelloAvalonia.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private bool _showForm;
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

    public MainWindowViewModel()
    {
        ShowFormCommand = new RelayCommand(() => ShowForm = true);
        AddItemCommand = new RelayCommand(AddItem);
        RemoveItemCommand = new RelayCommand<InvoiceItem>(RemoveItem);
        ChooseLogoCommand = new RelayCommand(ChooseLogo);
        RemoveLogoCommand = new RelayCommand(RemoveLogo);
        OpenSettingsCommand = new RelayCommand(OpenSettings);
        LoadProductCountCommand = new RelayCommand(async () => await LoadProductCountAsync(), () => IsInteractive);

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

    public RelayCommand ShowFormCommand { get; }
    public RelayCommand AddItemCommand { get; }
    public RelayCommand<InvoiceItem> RemoveItemCommand { get; }
    public RelayCommand ChooseLogoCommand { get; }
    public RelayCommand RemoveLogoCommand { get; }
    public RelayCommand OpenSettingsCommand { get; }
    public RelayCommand LoadProductCountCommand { get; }

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

    public bool IsInteractive => !_isInitializing && !_initializationFailed;
    
    public ObservableCollection<InvoiceItem> Items { get; } = new();

    public bool ShowForm
    {
        get => _showForm;
        set
        {
            if (_showForm != value)
            {
                _showForm = value;
                RaisePropertyChanged();
            }
        }
    }

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

    private void RecalculateTotals()
    {
        TotalHT = Items.Sum(i => i.TotalHT);
        TotalTTC = Items.Sum(i => i.TotalTTC);
        TotalTaxe = TotalTTC - TotalHT;
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
        if (result.Count > 0)
        {
            var filePath = result[0].Path.LocalPath;
            try
            {
                LogoImage = new Bitmap(filePath);
            }
            catch
            {
                // Failed to load image
                LogoImage = null;
            }
        }
    }

    private void RemoveLogo()
    {
        LogoImage = null;
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
}
