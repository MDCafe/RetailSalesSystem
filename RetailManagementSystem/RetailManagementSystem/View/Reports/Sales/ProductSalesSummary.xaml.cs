using RetailManagementSystem.ViewModel.Reports.Sales;
using System.Windows;

namespace RetailManagementSystem.View.Reports.Sales
{
    public partial class ProductSalesSummary : Window
    {
        public ProductSalesSummary()
        {
            InitializeComponent();
            DataContext = new ProductSalesSummaryViewModel();
        }
    }
}
