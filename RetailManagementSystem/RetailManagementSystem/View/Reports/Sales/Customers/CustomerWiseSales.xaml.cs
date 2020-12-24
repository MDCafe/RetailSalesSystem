using RetailManagementSystem.ViewModel.Reports;
using System.Windows;

namespace RetailManagementSystem.View.Reports.Sales.Customers
{
    public partial class CustomerWiseSales : Window
    {
        public CustomerWiseSales(bool showRestrictedCustomers)
        {
            InitializeComponent();
            DataContext = new CustomerWiseSalesViewModel(showRestrictedCustomers);
        }
    }
}
