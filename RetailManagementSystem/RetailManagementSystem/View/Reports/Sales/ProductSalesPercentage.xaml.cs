using RetailManagementSystem.ViewModel.Reports.Sales;
using System.Windows;

namespace RetailManagementSystem.View.Reports.Sales
{
    public partial class ProductSalesPercentage : Window
    {
        public ProductSalesPercentage()
        {
            InitializeComponent();
            this.DataContext = new ProductWiseSalesPercentViewModel(); ;
        }
    }
}
