using Avalonia.Controls;
using AroniumFactures.ViewModels;

namespace AroniumFactures.UserControls
{
    public partial class QuotationUserControl : UserControl
    {
        public QuotationUserControl()
        {
            InitializeComponent();
        }

        public QuotationUserControl(QuotationViewModel viewModel) : this()
        {
            DataContext = viewModel;
        }
    }
}


