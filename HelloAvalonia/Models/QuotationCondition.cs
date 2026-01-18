using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace AroniumFactures.Models;

public class QuotationCondition : INotifyPropertyChanged
{
    private string _title = string.Empty;
    private string _content = string.Empty;

    public string Title
    {
        get => _title;
        set
        {
            if (_title != value)
            {
                _title = value;
                RaisePropertyChanged();
            }
        }
    }

    public string Content
    {
        get => _content;
        set
        {
            if (_content != value)
            {
                _content = value;
                RaisePropertyChanged();
            }
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected void RaisePropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

