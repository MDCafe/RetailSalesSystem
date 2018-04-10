using RetailManagementSystem.ViewModel.Reports.Accounts;
using System.Windows;

namespace RetailManagementSystem.View.Reports.Accounts
{
    public partial class CustomerPaymentDetailsReport : Window
    {
        public CustomerPaymentDetailsReport(bool showRestrictedCustomers)
        {
            InitializeComponent();  
            DataContext = new CustomerPaymentDetailsRptViewModel(showRestrictedCustomers);
        }
    }
}
