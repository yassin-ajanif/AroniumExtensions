using Avalonia.Controls;
using Avalonia.Input;

namespace AroniumFactures.UserControls;

public partial class FactureView : UserControl
{
    public FactureView()
    {
        InitializeComponent();
    }

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        base.OnPointerPressed(e);
        // Remove focus from ICE TextBox when clicking outside
        if (IceNumberTextBox != null && IceNumberTextBox.IsFocused)
        {
            var source = e.Source;
            if (source != IceNumberTextBox && source is Control c && !IsDescendantOf(IceNumberTextBox, c))
            {
                IceNumberTextBox.IsEnabled = false;
                IceNumberTextBox.IsEnabled = true;
            }
        }
    }

    private static bool IsDescendantOf(Control ancestor, Control descendant)
    {
        var parent = descendant.Parent;
        while (parent != null)
        {
            if (parent == ancestor) return true;
            parent = parent.Parent;
        }
        return false;
    }
}
