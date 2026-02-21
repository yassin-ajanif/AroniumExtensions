using Avalonia.Controls;
using AroniumFactures.ViewModels;

namespace AroniumFactures.UserControls
{
    public partial class BonDeLivraisonUserControl : UserControl
    {
        public BonDeLivraisonUserControl()
        {
            InitializeComponent();
        }

        public BonDeLivraisonUserControl(BonDeLivraisonViewModel viewModel) : this()
        {
            DataContext = viewModel;
        }
    }
}



