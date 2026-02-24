using System;
using System.Globalization;
using Avalonia.Data.Converters;
using AroniumFactures.ViewModels;

namespace AroniumFactures.Helpers;

public class SidebarPageToBoolConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not SidebarPage page)
            return false;
        if (parameter is SidebarPage target)
            return page == target;
        if (parameter is string s && Enum.TryParse<SidebarPage>(s, out var parsed))
            return page == parsed;
        return false;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotImplementedException();
}
