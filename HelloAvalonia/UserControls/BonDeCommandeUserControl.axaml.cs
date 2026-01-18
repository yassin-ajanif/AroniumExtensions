using Avalonia.Controls;
using AroniumFactures.ViewModels;

namespace AroniumFactures.UserControls
{
    public partial class BonDeCommandeUserControl : UserControl
    {
        public BonDeCommandeUserControl()
        {
            InitializeComponent();
        }

        public BonDeCommandeUserControl(BonDeCommandeViewModel viewModel) : this()
        {
            DataContext = viewModel;
        }
    }
}

