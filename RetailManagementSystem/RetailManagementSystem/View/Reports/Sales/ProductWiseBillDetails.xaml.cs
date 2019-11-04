using RetailManagementSystem.ViewModel.Reports.Sales;
using System.Windows;

namespace RetailManagementSystem.View.Reports.Sales
{
    public partial class ProductWiseBillDetails : Window
    {
        public ProductWiseBillDetails()
        {
            InitializeComponent();
            this.DataContext = new ProductWiseBillDetailsViewModel(false); ;
        }
    }
}
