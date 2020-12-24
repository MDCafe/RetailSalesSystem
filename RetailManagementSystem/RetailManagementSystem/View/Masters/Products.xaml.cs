using RetailManagementSystem.ViewModel.Masters;
using System.Windows;

namespace RetailManagementSystem.View.Masters
{
    /// <summary>
    /// Interaction logic for Customer.xaml
    /// </summary>
    public partial class Products : Window
    {

        public Products()
        {
            InitializeComponent();
            DataContext = new ProductsViewModel();
        }
    }
}
