using RetailManagementSystem.ViewModel.Reports;
using System.Windows;

namespace RetailManagementSystem.View.Reports.Sales
{
    public partial class SalesSummary : Window
    {
        public SalesSummary(bool showRestrictedCustomers)
        {
            InitializeComponent();
            DataContext = new SalesSummaryViewModel(showRestrictedCustomers);
        }
    }
}
