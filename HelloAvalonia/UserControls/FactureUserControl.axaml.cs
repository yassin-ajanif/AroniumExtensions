using Avalonia.Controls;
using AroniumFactures.ViewModels;

namespace AroniumFactures.UserControls
{
    public partial class FactureUserControl : UserControl
    {
        public FactureUserControl()
        {
            InitializeComponent();
        }

        public FactureUserControl(FactureViewModel viewModel) : this()
        {
            DataContext = viewModel;
        }
    }
}

