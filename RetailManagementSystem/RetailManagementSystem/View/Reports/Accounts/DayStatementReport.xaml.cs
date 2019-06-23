using RetailManagementSystem.ViewModel.Reports.Accounts;
using System.Windows;

namespace RetailManagementSystem.View.Reports.Accounts
{
    public partial class DayStatementReport : Window
    {
        public DayStatementReport(bool showRestrictedCustomers)
        {
            InitializeComponent();
            DataContext = new DayStatementReportViewModel(showRestrictedCustomers);
        }
    }
}
