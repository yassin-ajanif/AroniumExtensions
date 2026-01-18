using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AroniumFactures.Models;

public class InvoiceItem : INotifyPropertyChanged
{
    private string _reference = string.Empty;
    private string _designation = string.Empty;
    private string _unit = "U";
    private decimal _quantity = 1.0m;
    private decimal _unitPrice = 0m;
    private decimal _tvaRate = 20m;
    private decimal _itemDiscount = 0m;
    private int _itemDiscountType = 0;
    private decimal _taxAmount = 0m;
    private decimal _totalBeforeDiscount = 0m;
    private decimal _totalAfterDiscount = 0m;

    public string Reference
    {
        get => _reference;
        set
        {
            if (_reference != value)
            {
                _reference = value;
                OnPropertyChanged();
            }
        }
    }

    public string Designation
    {
        get => _designation;
        set
        {
            if (_designation != value)
            {
                _designation = value;
                OnPropertyChanged();
            }
        }
    }

    public string Unit
    {
        get => _unit;
        set
        {
            if (_unit != value)
            {
                _unit = value;
                OnPropertyChanged();
            }
        }
    }

    public decimal Quantity
    {
        get => _quantity;
        set
        {
            if (_quantity != value)
            {
                _quantity = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(TotalHT));
                OnPropertyChanged(nameof(TotalTTC));
            }
        }
    }

    public decimal UnitPrice
    {
        get => _unitPrice;
        set
        {
            if (_unitPrice != value)
            {
                _unitPrice = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(TotalHT));
                OnPropertyChanged(nameof(TotalTTC));
            }
        }
    }

    public decimal TvaRate
    {
        get => _tvaRate;
        set
        {
            if (_tvaRate != value)
            {
                _tvaRate = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(TotalTTC));
            }
        }
    }

    public decimal ItemDiscount
    {
        get => _itemDiscount;
        set
        {
            if (_itemDiscount != value)
            {
                _itemDiscount = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ItemDiscountDisplay));
            }
        }
    }

    public int ItemDiscountType
    {
        get => _itemDiscountType;
        set
        {
            if (_itemDiscountType != value)
            {
                _itemDiscountType = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ItemDiscountDisplay));
            }
        }
    }

    public string ItemDiscountDisplay
    {
        get
        {
            if (_itemDiscountType == 0) // Percentage (0 = percentage, 1 = fixed)
                return $"{_itemDiscount:F2}%";
            else // Fixed value
                return $"{_itemDiscount:F2}";
        }
    }

    public decimal TaxAmount
    {
        get => _taxAmount;
        set
        {
            if (_taxAmount != value)
            {
                _taxAmount = value;
                OnPropertyChanged();
            }
        }
    }

    public decimal TotalHT => _quantity * _unitPrice;

    public decimal TotalTTC
    {
        get => _totalBeforeDiscount;
        set
        {
            if (_totalBeforeDiscount != value)
            {
                _totalBeforeDiscount = value;
                OnPropertyChanged();
            }
        }
    }

    public decimal TotalAfterDiscount
    {
        get => _totalAfterDiscount;
        set
        {
            if (_totalAfterDiscount != value)
            {
                _totalAfterDiscount = value;
                OnPropertyChanged();
            }
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

