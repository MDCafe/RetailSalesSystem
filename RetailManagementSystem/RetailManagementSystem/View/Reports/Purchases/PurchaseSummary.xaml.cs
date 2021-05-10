using RetailManagementSystem.ViewModel.Reports.Purhcases;
using System.Windows;

namespace RetailManagementSystem.View.Reports.Purchases
{
    public partial class PurchaseSummary : Window
    {
        public PurchaseSummary()
        {
            InitializeComponent();
            this.DataContext = new PurchaseSummaryViewModel(false, 0);

        }
    }
}
