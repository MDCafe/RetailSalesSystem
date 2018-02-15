using RetailManagementSystem.ViewModel.Masters;
using System.Windows;

namespace RetailManagementSystem.View.Masters
{
    /// <summary>
    /// Interaction logic for Customer.xaml
    /// </summary>
    public partial class Customer : Window
    {
        
        public Customer()
        {
            InitializeComponent();
            DataContext = new CustomerViewModel();
        }
    }
}
