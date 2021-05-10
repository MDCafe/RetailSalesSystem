using RetailManagementSystem.ViewModel.Reports.Stock;
using System.Windows;

namespace RetailManagementSystem.View.Reports.Stock
{
    public partial class StockReport : Window
    {
        public StockReport()
        {
            InitializeComponent();
            this.DataContext = new StockReportViewModel(false);
        }
    }
}
