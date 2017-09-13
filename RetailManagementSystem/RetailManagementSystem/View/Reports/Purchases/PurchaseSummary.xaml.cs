using RetailManagementSystem.ViewModel.Reports.Purhcases;
using System.Windows;

namespace RetailManagementSystem.View.Reports.Purchases
{
    public partial class PurchaseSummary : Window
    {
        PurchaseSummaryViewModel _purchaseSummaryViewModel = new PurchaseSummaryViewModel(false);

        public PurchaseSummary(bool showRestrictedCustomers)
        {
            InitializeComponent();
            this.DataContext = _purchaseSummaryViewModel;
            
        }
    }
}
