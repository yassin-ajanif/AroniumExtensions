using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using AroniumFactures.ViewModels;

namespace AroniumFactures;

public partial class CompanionAppPathDialog : Window
{
    public CompanionAppPathDialog()
    {
        InitializeComponent();
        DataContext = new CompanionAppPathDialogViewModel();
    }

    private void Cancel_Click(object? sender, RoutedEventArgs e)
    {
        Close(false);
    }

    private async void Submit_Click(object? sender, RoutedEventArgs e)
    {
        if (DataContext is CompanionAppPathDialogViewModel vm)
        {
            await vm.SavePathAsync();
        }
        Close(true);
    }
}
