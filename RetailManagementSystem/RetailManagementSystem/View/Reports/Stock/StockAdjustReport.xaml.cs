using RetailManagementSystem.ViewModel.Reports.Stock;
using System.Windows;

namespace RetailManagementSystem.View.Reports.Stock
{
    public partial class StockAdjustReport : Window
    {
        public StockAdjustReport()
        {
            InitializeComponent();
            DataContext = new StockAdjustReportViewModel();
        }
    }
}
