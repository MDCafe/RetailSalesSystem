using RetailManagementSystem.ViewModel.Masters;
using System.Windows;

namespace RetailManagementSystem.View.Masters
{
    /// <summary>
    /// Interaction logic for Customer.xaml
    /// </summary>
    public partial class Companies : Window
    {

        public Companies()
        {
            InitializeComponent();
            DataContext = new CompanyViewModel();
        }
    }
}
