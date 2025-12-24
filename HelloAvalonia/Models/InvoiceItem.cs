using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace HelloAvalonia.Models;

public class InvoiceItem : INotifyPropertyChanged
{
    private string _reference = string.Empty;
    private string _designation = string.Empty;
    private double _quantity = 1.0;
    private decimal _unitPrice = 0m;
    private decimal _tvaRate = 20m;

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

    public double Quantity
    {
        get => _quantity;
        set
        {
            if (Math.Abs(_quantity - value) > double.Epsilon)
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

    public decimal TotalHT => (decimal)_quantity * _unitPrice;

    public decimal TotalTTC => TotalHT * (1 + _tvaRate / 100);

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

