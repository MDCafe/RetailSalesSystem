using RetailManagementSystem.ViewModel;
using RetailManagementSystem.ViewModel.Reports;
using RetailManagementSystem.ViewModel.Sales;
using System.Windows;
using System.Windows.Controls;

namespace RetailManagementSystem.View.Reports.Sales
{
    public partial class SalesSummary : Window
    {
        SalesSummaryViewModel _salesSummaryViewModel = new SalesSummaryViewModel(false);

        public SalesSummary(bool showRestrictedCustomers)
        {
            InitializeComponent();
            this.DataContext = _salesSummaryViewModel;
            
        }
    }
}
