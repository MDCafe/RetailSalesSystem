using RetailManagementSystem.ViewModel.Reports.Stock;
using System.Windows;

namespace RetailManagementSystem.View.Reports.Stock
{
    public partial class StockBalanceReport : Window
    {
        public StockBalanceReport()
        {
            InitializeComponent();
            DataContext = new StockBalanceReportViewModel(false);
        }
    }
}
