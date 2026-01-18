using System;
using System.Collections.ObjectModel;
using AroniumFactures.Models;
using AroniumFactures;

namespace AroniumFactures.ViewModels;

public class QuotationViewModel : ViewModelBase
{
    // Client Information
    private string _clientName = "MAZARS BUSINESS SERVICES";
    private string _clientAddress = "101, Boulevard Abdelmoumen 20100 Casablanca, Maroc";
    private string _clientSite = string.Empty;

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

    public ObservableCollection<InvoiceItem> Items { get; } = new();
    public ObservableCollection<QuotationCondition> Conditions { get; } = new();

    public RelayCommand AddConditionCommand { get; }
    public RelayCommand<QuotationCondition> RemoveConditionCommand { get; }

    public QuotationViewModel()
    {
        AddConditionCommand = new RelayCommand(AddCondition);
        RemoveConditionCommand = new RelayCommand<QuotationCondition>(RemoveCondition);
        
        // Initialize with demo data from the image
        LoadDemoData();
        LoadDefaultConditions();
        RecalculateTotals();
    }

    private void LoadDefaultConditions()
    {
        Conditions.Clear();
        
        Conditions.Add(new QuotationCondition
        {
            Title = "NON COMPRIS",
            Content = "Tous ce qui n'apparaît pas dans le présent devis"
        });
        
        Conditions.Add(new QuotationCondition
        {
            Title = "DUREE DES TRAVAUX",
            Content = "10 jours"
        });
        
        Conditions.Add(new QuotationCondition
        {
            Title = "VALIDITE DE DEVIS",
            Content = "1 mois"
        });
        
        Conditions.Add(new QuotationCondition
        {
            Title = "PAIEMENT",
            Content = "50% à la commande par chèque.\n50% au cour des travaux"
        });
    }

    private void AddCondition()
    {
        int nextNumber = Conditions.Count + 1;
        Conditions.Add(new QuotationCondition
        {
            Title = $"CONDITION {nextNumber}",
            Content = string.Empty
        });
    }

    private void RemoveCondition(QuotationCondition? condition)
    {
        if (condition != null)
        {
            Conditions.Remove(condition);
        }
    }

    private void LoadDemoData()
    {
        Items.Clear();
        
        Items.Add(new InvoiceItem
        {
            Reference = "01",
            Designation = "Split système gainable 48 000 BTU",
            Quantity = 3,
            UnitPrice = 31071.60m,
            TvaRate = 20m
        });

        Items.Add(new InvoiceItem
        {
            Reference = "02",
            Designation = "Gaine de souflage calorifugee Ø 200",
            Quantity = 212,
            UnitPrice = 98.64m,
            TvaRate = 20m
        });

        Items.Add(new InvoiceItem
        {
            Reference = "03",
            Designation = "Diffuseur de soufflage 600x600",
            Quantity = 21,
            UnitPrice = 752.13m,
            TvaRate = 20m
        });

        Items.Add(new InvoiceItem
        {
            Reference = "04",
            Designation = "Plenium de soufflage en fiber",
            Quantity = 3,
            UnitPrice = 656.23m,
            TvaRate = 20m
        });

        Items.Add(new InvoiceItem
        {
            Reference = "05",
            Designation = "Condensat en pvc",
            Quantity = 1,
            UnitPrice = 2471.48m,
            TvaRate = 20m
        });

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

        // Subscribe to existing items
        foreach (var item in Items)
        {
            item.PropertyChanged += (sender, args) => RecalculateTotals();
        }
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
}

